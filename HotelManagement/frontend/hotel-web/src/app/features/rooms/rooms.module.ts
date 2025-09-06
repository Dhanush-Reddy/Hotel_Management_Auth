import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../shared/material.module';

import { RoomsRoutingModule } from './rooms-routing.module';
import { RoomsListComponent } from './pages/rooms-list/rooms-list.component';
import { RoomFormComponent } from './pages/room-form/room-form.component';

@NgModule({
  declarations: [RoomsListComponent, RoomFormComponent],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MaterialModule, RoomsRoutingModule]
})
export class RoomsModule {}
