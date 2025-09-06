import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../shared/material.module';

import { GuestsRoutingModule } from './guests-routing.module';
import { GuestsListComponent } from './pages/guests-list/guests-list.component';
import { GuestFormComponent } from './pages/guest-form/guest-form.component';

@NgModule({
  declarations: [GuestsListComponent, GuestFormComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule, GuestsRoutingModule]
})
export class GuestsModule {}

