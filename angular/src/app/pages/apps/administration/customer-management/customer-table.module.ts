import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomerTableRoutingModule } from './customer-table-routing.module';
import { CustomerTableComponent } from './customer-table.component';
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
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { PageLayoutModule } from 'src/@vex/components/page-layout/page-layout.module';
import { BreadcrumbsModule } from 'src/@vex/components/breadcrumbs/breadcrumbs.module';
import { CustomerCreateUpdateModule } from './customer-create-update/customer-create-update.module';
import { ContactCreateUpdateModule } from './customer-contact/contact-create-update/contact-create-update.module';
import { LocationCreateUpdateModule } from './customer-location/location-create-update/location-create-update.module';
import { MatTabsModule } from '@angular/material/tabs';


@NgModule({
  declarations: [CustomerTableComponent],
  imports: [
    CommonModule,
    CustomerTableRoutingModule,
    PageLayoutModule,
    BreadcrumbsModule,
    CustomerCreateUpdateModule,
    ContactCreateUpdateModule,
    LocationCreateUpdateModule,
    MatPaginatorModule,
    MatTableModule,
    MatSortModule,
    MatCheckboxModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    FormsModule,
    MatTooltipModule,
    ReactiveFormsModule,
    MatSelectModule,
    MatButtonToggleModule,
    MatFormFieldModule,
    MatInputModule
  ]
})
export class CustomerTableModule {
}
