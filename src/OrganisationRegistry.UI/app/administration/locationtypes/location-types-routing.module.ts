﻿import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { LocationTypeDetailComponent } from './detail';
import { LocationTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/locationtypes',
    component: LocationTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Locatie types'
    }
  },
  {
    path: 'administration/locationtypes/create',
    component: LocationTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuw locatie type'
    }
  },
  {
    path: 'administration/locationtypes/:id',
    component: LocationTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken locatie type'
    }
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class LocationTypesRoutingModule { }
