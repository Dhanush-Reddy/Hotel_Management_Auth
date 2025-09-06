import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';

const routes: Routes = [
  { path: 'login', loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule) },
  { path: 'rooms', loadChildren: () => import('./features/rooms/rooms.module').then(m => m.RoomsModule) },
  { path: 'guests', loadChildren: () => import('./features/guests/guests.module').then(m => m.GuestsModule) },
  { path: '', redirectTo: 'rooms', pathMatch: 'full' },
  { path: 'bookings', loadChildren: () => import('./features/bookings/bookings.module').then(m => m.BookingsModule) },
  { path: 'invoices', loadChildren: () => import('./features/invoices/invoices.module').then(m => m.InvoicesModule) },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
