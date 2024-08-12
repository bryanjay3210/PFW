import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { PageLayoutModule } from 'src/@vex/components/page-layout/page-layout.module';
import { BreadcrumbsModule } from 'src/@vex/components/breadcrumbs/breadcrumbs.module';
import { PaymentTableComponent } from './payment-table.component';
import { PaymentCreateUpdateModule } from './payment-create-update/payment-create-update.module';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { PaymentTableRoutingModule } from './payment-table-routing.module';
import { PaymentAddCreditModule } from './payment-add-credit/payment-add-credit.module';
import { PaymentCreditMemoModule } from './payment-credit-memo/payment-credit-memo.module';
import { PaymentCustomerListModule } from './payment-customer-list/payment-customer-list.module';
import { MatDatepickerModule } from '@angular/material/datepicker';

@NgModule({
  declarations: [PaymentTableComponent],
  imports: [
    CommonModule,
    MatExpansionModule,
    MatButtonModule,
    MatIconModule,
    PaymentTableRoutingModule,
    PaymentCreateUpdateModule,
    PaymentAddCreditModule,
    PaymentCreditMemoModule,
    PaymentCustomerListModule,
    PageLayoutModule,
    BreadcrumbsModule,
    MatPaginatorModule,
    MatTableModule,
    MatSortModule,
    MatCheckboxModule,
    MatMenuModule,
    FormsModule,
    MatTooltipModule,
    ReactiveFormsModule,
    MatSelectModule,
    MatButtonToggleModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule,
    MatDatepickerModule
  ]
})
export class PaymentTableModule {
}
