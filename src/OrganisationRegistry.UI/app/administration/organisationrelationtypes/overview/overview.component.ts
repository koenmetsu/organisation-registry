import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationRelationType,
  OrganisationRelationTypeService,
  OrganisationRelationTypeFilter
} from 'services/organisationrelationtypes';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class OrganisationRelationTypeOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public organisationRelationTypes: PagedResult<OrganisationRelationType> = new PagedResult<OrganisationRelationType>();

  private filter: OrganisationRelationTypeFilter = new OrganisationRelationTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private organisationRelationTypeService: OrganisationRelationTypeService) { }

  ngOnInit() {
    this.loadOrganisationRelationTypes();
  }

  search(event: SearchEvent<OrganisationRelationTypeFilter>) {
    this.filter = event.fields;
    this.loadOrganisationRelationTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadOrganisationRelationTypes(event);
  }

  private loadOrganisationRelationTypes(event?: PagedEvent) {
    this.isLoading = true;
    let organisationRelationTypes = (event === undefined)
      ? this.organisationRelationTypeService.getOrganisationRelationTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationRelationTypeService.getOrganisationRelationTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    organisationRelationTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newOrganisationRelationTypes => this.organisationRelationTypes = newOrganisationRelationTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Organisatie relatie types kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          )));
  }
}
