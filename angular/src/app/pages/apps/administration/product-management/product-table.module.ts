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
import { ProductTableComponent } from './product-table.component';
import { ProductTableRoutingModule } from './product-table-routing.module';
import { ProductCreateUpdateModule } from './product-create-update/product-create-update.module';
import { PartsCatalogCreateUpdateModule } from './product-parts-catalog/parts-catalog-create-update/parts-catalog-create-update.module';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { PartsLinkCreateUpdateModule } from './product-parts-link/parts-link-create-update/parts-link-create-update.module';
import { VendorCatalogCreateUpdateModule } from './product-vendor-catalog/vendor-catalog-create-update/vendor-catalog-create-update.module';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { PartsLocationCreateUpdateModule } from './parts-location-create-update/parts-location-create-update.module';
import { NgxBarcodeModule } from '@greatcloak/ngx-barcode';

@NgModule({
  declarations: [ProductTableComponent],
  imports: [
    NgxBarcodeModule,
    CommonModule,
    MatExpansionModule,
    MatButtonModule,
    MatIconModule,
    PartsLocationCreateUpdateModule,
    ProductTableRoutingModule,
    ProductCreateUpdateModule,
    PartsCatalogCreateUpdateModule,
    PartsLinkCreateUpdateModule,
    VendorCatalogCreateUpdateModule,
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
    MatSlideToggleModule
  ]
})
export class ProductTableModule {
}
