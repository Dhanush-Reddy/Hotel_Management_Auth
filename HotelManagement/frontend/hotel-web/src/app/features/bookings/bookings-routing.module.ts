import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BookingsListComponent } from './pages/bookings-list/bookings-list.component';
import { BookingFormComponent } from './pages/booking-form/booking-form.component';
import { BookingViewComponent } from './pages/booking-view/booking-view.component';
import { AuthGuard } from '../../core/guards/auth.guard';

const routes: Routes = [
  { path: '', component: BookingsListComponent, canActivate:[AuthGuard] },
  { path: 'new', component: BookingFormComponent, canActivate:[AuthGuard] },
  { path: ':id', component: BookingViewComponent, canActivate:[AuthGuard] },
  { path: ':id/edit', component: BookingFormComponent, canActivate:[AuthGuard] }
];

@NgModule({ imports:[RouterModule.forChild(routes)], exports:[RouterModule] })
export class BookingsRoutingModule {}
