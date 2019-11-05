import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { AuthService, OidcService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationOrganisationClassificationListItem,
  OrganisationOrganisationClassificationService,
  OrganisationOrganisationClassificationFilter
} from 'services/organisationorganisationclassifications';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationOrganisationClassificationsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationOrganisationClassifications: PagedResult<OrganisationOrganisationClassificationListItem>;
  public canEditOrganisation: Observable<boolean>;

  private filter: OrganisationOrganisationClassificationFilter = new OrganisationOrganisationClassificationFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie classificaties');
  private organisationId: string;
  private currentSortBy: string = 'organisationClassificationTypeName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationOrganisationClassificationService: OrganisationOrganisationClassificationService,
    private oidcService: OidcService,
    private alertService: AlertService
  ) {
    this.organisationOrganisationClassifications = new PagedResult<OrganisationOrganisationClassificationListItem>();
  }

  ngOnInit() {
    this.canEditOrganisation = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.canEditOrganisation = this.oidcService.canEditOrganisation(this.organisationId);
      this.loadOrganisationClassifications();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationOrganisationClassificationFilter>) {
    this.filter = event.fields;
    this.loadOrganisationClassifications();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadOrganisationClassifications(event);
  }

  private loadOrganisationClassifications(event?: PagedEvent) {
    this.isLoading = true;
    let organisationClassifications = (event === undefined)
      ? this.organisationOrganisationClassificationService.getOrganisationOrganisationClassifications(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationOrganisationClassificationService.getOrganisationOrganisationClassifications(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    organisationClassifications
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationOrganisationClassifications = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
