import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  PersonListItem,
  PersonService,
  PersonFilter
} from 'services/people';


@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class PersonOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public people: PagedResult<PersonListItem> = new PagedResult<PersonListItem>();

  private filter: PersonFilter = new PersonFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private personService: PersonService) { }

  ngOnInit() {
    this.loadPersons();
  }

  search(event: SearchEvent<PersonFilter>) {
    this.filter = event.fields;
    this.loadPersons();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadPersons(event);
  }

  private loadPersons(event?: PagedEvent) {
    this.isLoading = true;
    let people = (event === undefined)
      ? this.personService.getPeople(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.personService.getPeople(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    people
      .finally(() => this.isLoading = false)
      .subscribe(
        newPeople => this.people = newPeople,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Personen kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          )));
  }
}
