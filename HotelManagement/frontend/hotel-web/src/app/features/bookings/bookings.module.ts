import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../shared/material.module';
import { BookingsRoutingModule } from './bookings-routing.module';
import { BookingsListComponent } from './pages/bookings-list/bookings-list.component';
import { BookingFormComponent } from './pages/booking-form/booking-form.component';
import { BookingViewComponent } from './pages/booking-view/booking-view.component';

@NgModule({
  declarations: [BookingsListComponent, BookingFormComponent, BookingViewComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule, BookingsRoutingModule]
})
export class BookingsModule {}
