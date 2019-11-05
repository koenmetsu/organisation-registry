import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationClassificationType,
  OrganisationClassificationTypeService,
  OrganisationClassificationTypeFilter
} from 'services/organisationclassificationtypes';


@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class OrganisationClassificationTypeOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public organisationClassificationTypes: PagedResult<OrganisationClassificationType> = new PagedResult<OrganisationClassificationType>();

  private filter: OrganisationClassificationTypeFilter = new OrganisationClassificationTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private organisationClassificationTypeService: OrganisationClassificationTypeService) { }

  ngOnInit() {
    this.loadOrganisationClassificationTypes();
  }

  search(event: SearchEvent<OrganisationClassificationTypeFilter>) {
    this.filter = event.fields;
    this.loadOrganisationClassificationTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadOrganisationClassificationTypes(event);
  }

  private loadOrganisationClassificationTypes(event?: PagedEvent) {
    this.isLoading = true;
    let organisationClassificationTypes = (event === undefined)
      ? this.organisationClassificationTypeService.getOrganisationClassificationTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationClassificationTypeService.getOrganisationClassificationTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    organisationClassificationTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newOrganisationClassificationTypes => this.organisationClassificationTypes = newOrganisationClassificationTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Organisatie classificatietypes kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          )));
  }
}
