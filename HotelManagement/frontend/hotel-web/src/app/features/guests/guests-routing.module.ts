import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GuestsListComponent } from './pages/guests-list/guests-list.component';
import { GuestFormComponent } from './pages/guest-form/guest-form.component';
import { AuthGuard } from '../../core/guards/auth.guard';

const routes: Routes = [
  { path: '', component: GuestsListComponent, canActivate:[AuthGuard] },
  { path: 'new', component: GuestFormComponent, canActivate:[AuthGuard] },
  { path: ':id/edit', component: GuestFormComponent, canActivate:[AuthGuard] }
];

@NgModule({ imports: [RouterModule.forChild(routes)], exports: [RouterModule] })
export class GuestsRoutingModule {}
