import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InvoicesListComponent } from './pages/invoices-list/invoices-list.component';
import { InvoiceViewComponent } from './pages/invoice-view/invoice-view.component';
import { AuthGuard } from '../../core/guards/auth.guard';

const routes: Routes = [
  { path: '', component: InvoicesListComponent, canActivate:[AuthGuard] },
  { path: ':id', component: InvoiceViewComponent, canActivate:[AuthGuard] }
];

@NgModule({ imports:[RouterModule.forChild(routes)], exports:[RouterModule] })
export class InvoicesRoutingModule {}
