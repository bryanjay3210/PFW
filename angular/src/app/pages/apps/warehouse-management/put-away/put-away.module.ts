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
import { PutAwayComponent } from './put-away.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { PutAwayRoutingModule } from './put-away-routing.module';
import { NgxBarcodeModule } from '@greatcloak/ngx-barcode';
import { HttpClientModule } from '@angular/common/http';
import { MatTabsModule } from '@angular/material/tabs';
import { BackOrderModule } from '../../administration/order-management/back-order/back-order.module';
import { CreditMemoModule } from '../../administration/order-management/credit-memo/credit-memo.module';
import { CustomerListModule } from '../../administration/order-management/customer-list/customer-list.module';
import { LocationListModule } from '../../administration/order-management/location-list/location-list.module';
import { OrderCreateUpdateModule } from '../../administration/order-management/order-create-update/order-create-update.module';
import { OrderTableRoutingModule } from '../../administration/order-management/order-table-routing.module';
import { ProductListModule } from '../../administration/order-management/product-list/product-list.module';
import { VendorInputDialogModule } from '../../administration/order-management/vendor-input-dialog/vendor-input-dialog.module';
import { SearchProductListModule } from './search-product-list/search-product-list.module';


@NgModule({
  declarations: [PutAwayComponent],
  imports: [
    CommonModule,
    MatExpansionModule,
    MatButtonModule,
    MatIconModule,
    SearchProductListModule,
    PutAwayRoutingModule,
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

    NgxBarcodeModule,
    HttpClientModule,
    BackOrderModule,
    OrderTableRoutingModule,
    OrderCreateUpdateModule,
    CreditMemoModule,
    CustomerListModule,
    LocationListModule,
    ProductListModule,
    VendorInputDialogModule,
    MatTabsModule
  ]
})
export class PutAwayModule {
}
