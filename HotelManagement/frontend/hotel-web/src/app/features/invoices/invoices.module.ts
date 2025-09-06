import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialModule } from '../../shared/material.module';
import { InvoicesRoutingModule } from './invoices-routing.module';
import { InvoicesListComponent } from './pages/invoices-list/invoices-list.component';
import { InvoiceViewComponent } from './pages/invoice-view/invoice-view.component';

@NgModule({
  declarations: [InvoicesListComponent, InvoiceViewComponent],
  imports: [CommonModule, FormsModule, MaterialModule, InvoicesRoutingModule]
})
export class InvoicesModule {}
