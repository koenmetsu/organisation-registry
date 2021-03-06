import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { AuthService } from 'core/auth';
import { AlertService, AlertBuilder } from 'core/alert';

import { OrganisationService } from 'services/organisations';
import { PurposeService } from 'services/purposes';

import { SelectItem } from 'shared/components/form/form-group-select';

import { CreateChildAlerts } from './create.alerts';

import { CreateOrganisationFormValues } from './create-organisation-form';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class CreateOrganisationComponent implements OnInit {
  public isBusy: boolean = true;
  public organisation;
  public purposes: SelectItem[];

  private readonly createAlerts = new CreateChildAlerts();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationService: OrganisationService,
    private purposeService: PurposeService,
    private alertService: AlertService,
    private authService: AuthService) { }

  ngOnInit() {
    this.organisation = new CreateOrganisationFormValues();

    this.purposeService
      .getAllPurposes()
      .delay(5000)
      .finally(() => this.isBusy = this.purposes.length <= 0)
      .subscribe(
        allPurposes => {
          this.purposes = allPurposes.map(k => new SelectItem(k.id, k.name));
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Beleidsvelden konden niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de beleidsvelden. Probeer het later opnieuw.')
              .build()));
  }

  create(value: CreateOrganisationFormValues) {
    this.isBusy = true;
    this.organisationService.create(value)
      .finally(() => this.isBusy = false)
      .subscribe(
        result => this.onCreateSuccess(result, value),
        error => this.alertService.setAlert(this.createAlerts.saveError(error))
      );
  }

  private onCreateSuccess(result, value) {
    if (result) {
      this.authService.resetSecurityCache();

      this.router.navigate(['./../', value.id], { relativeTo: this.route });

      let organisationUrl = this.router.serializeUrl(
        this.router.createUrlTree(
          ['./../', value.id],
          { relativeTo: this.route }));

      this.alertService.setAlert(this.createAlerts.saveSuccess(value, organisationUrl));
    }
  }
}
