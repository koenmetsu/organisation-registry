﻿import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  ConfigurationValueListItem,
  ConfigurationValueService,
  ConfigurationValueFilter
} from 'services/configurationvalues';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class ConfigurationValueOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public configurationValues: PagedResult<ConfigurationValueListItem> = new PagedResult<ConfigurationValueListItem>();

  private filter: ConfigurationValueFilter = new ConfigurationValueFilter();
  private currentSortBy: string = 'key';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private configurationValueService: ConfigurationValueService) { }

  ngOnInit() {
    this.loadConfigurationValues();
  }

  search(event: SearchEvent<ConfigurationValueFilter>) {
    this.filter = event.fields;
    this.loadConfigurationValues();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadConfigurationValues(event);
  }

  private loadConfigurationValues(event?: PagedEvent) {
    this.isLoading = true;
    let configurationValues = (event === undefined)
      ? this.configurationValueService.getConfigurationValues(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.configurationValueService.getConfigurationValues(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    configurationValues
      .finally(() => this.isLoading = false)
      .subscribe(
        newConfigurationValues => this.configurationValues = newConfigurationValues,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Configuratiewaarden kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          )));
  }
}
