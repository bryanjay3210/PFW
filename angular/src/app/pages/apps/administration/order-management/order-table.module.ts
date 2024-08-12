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
import { OrderTableComponent } from './order-table.component';
import { OrderTableRoutingModule } from './order-table-routing.module';
import { OrderCreateUpdateModule } from './order-create-update/order-create-update.module';
import { CustomerListModule } from './customer-list/customer-list.module';
import { LocationListModule } from './location-list/location-list.module';
import { ProductListModule } from './product-list/product-list.module';
import { VendorInputDialogModule } from './vendor-input-dialog/vendor-input-dialog.module';
import { MatTabsModule } from '@angular/material/tabs';
import { PrintInvoiceLayoutComponent } from './print-invoice-layout/print-invoice-layout.component';
import { HttpClientModule } from '@angular/common/http';
import { PrintInvoiceComponent } from './print-invoice/print-invoice.component';
import { CreditMemoModule } from './credit-memo/credit-memo.module';
import { BackOrderModule } from './back-order/back-order.module';
import { NgxBarcodeModule } from '@greatcloak/ngx-barcode';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDialogModule } from '@angular/material/dialog';
import { CustomerEmailDialogModule } from './customer-email-dialog/customer-email-dialog.module';
import { DiscountModule } from './discount/discount.module';
import { OrderStatusUpdateModule } from './order-status-update/order-status-update.module';
import { RGAInspectedCodeModule } from './rga-inspected-code/rga-inspected-code.module';

@NgModule({
  declarations: [OrderTableComponent, PrintInvoiceLayoutComponent, PrintInvoiceComponent],
  imports: [
    NgxBarcodeModule,
    CommonModule,
    HttpClientModule,
    MatExpansionModule,
    MatButtonModule,
    MatIconModule,
    BackOrderModule,
    OrderTableRoutingModule,
    OrderCreateUpdateModule,
    RGAInspectedCodeModule,
    CreditMemoModule,
    DiscountModule,
    CustomerListModule,
    LocationListModule,
    ProductListModule,
    VendorInputDialogModule,
    CustomerEmailDialogModule,
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
    MatTabsModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule,
  ]
})
export class OrderTableModule {
}
