import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoomsListComponent } from './pages/rooms-list/rooms-list.component';
import { RoomFormComponent } from './pages/room-form/room-form.component';

const routes: Routes = [
  { path: '', component: RoomsListComponent },
  { path: 'new', component: RoomFormComponent },
  { path: ':id/edit', component: RoomFormComponent }
];

@NgModule({ imports: [RouterModule.forChild(routes)], exports: [RouterModule] })
export class RoomsRoutingModule {}
