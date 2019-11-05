﻿import { Component, OnInit, ViewChild, ElementRef, Renderer } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { Update, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { Role } from 'core/auth';

import { SelectItem } from 'shared/components/form/form-group-select';
import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import {
  UpdateBodyOrganisationRequest,
  BodyOrganisationService
} from 'services/bodyorganisations';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class BodyOrganisationsUpdateBodyOrganisationComponent implements OnInit {
  public form: FormGroup;
  public organisation: SearchResult;
  public isOrgaanBeheerder: boolean;

  private readonly updateAlerts = new UpdateAlertMessages('Organisatie');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private bodyOrganisationService: BodyOrganisationService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      bodyOrganisationId: ['', required],
      bodyId: ['', required],
      organisationId: ['', required],
      organisationName: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    let roles = this.route.snapshot.data['userRoles'];
    this.isOrgaanBeheerder = roles.indexOf(Role.OrgaanBeheerder) !== -1;

    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let bodyOrganisationId = res[1]['id'];

        this.bodyOrganisationService
          .get(orgId, bodyOrganisationId)
          .subscribe(
            item => this.setForm(item),
            error => this.handleError(error));
      });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateBodyOrganisationRequest) {
    this.form.disable();

    this.bodyOrganisationService.update(value.bodyId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./../..'], { relativeTo: this.route });
            this.handleSaveSuccess();
          }
        },
        error => this.handleSaveError(error)
      );
  }

  private setForm(bodyOrganisation) {
    if (bodyOrganisation) {
      this.form.setValue(bodyOrganisation);
      this.organisation = new SearchResult(bodyOrganisation.organisationId, bodyOrganisation.organisationName);
    }

    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Organisatie kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de organisatie. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Organisatie bijgewerkt!')
        .withMessage('Organisatie is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Organisatie kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
