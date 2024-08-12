import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { CustomerCreateUpdateComponent } from './customer-create-update.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { RouterModule } from '@angular/router';
import { QuicklinkModule } from 'ngx-quicklink';
import { MatTabsModule } from '@angular/material/tabs';
import { PageLayoutModule } from 'src/@vex/components/page-layout/page-layout.module';
import { BreadcrumbsModule } from 'src/@vex/components/breadcrumbs/breadcrumbs.module';
import { SecondaryToolbarModule } from 'src/@vex/components/secondary-toolbar/secondary-toolbar.module';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ContactTableComponent } from '../customer-contact/contact-table.component';
import { LocationTableComponent } from '../customer-location/location-table.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatRadioModule,
    MatSelectModule,
    MatMenuModule,
    MatDividerModule,
    RouterModule,
    QuicklinkModule,
    MatTabsModule,
    PageLayoutModule,
    BreadcrumbsModule,
    SecondaryToolbarModule,
    MatDatepickerModule,
    MatAutocompleteModule,
    MatNativeDateModule, 
    MatPaginatorModule,
    MatTableModule,
    MatSortModule,
    MatCheckboxModule,
    FormsModule,
    MatTooltipModule,
    MatButtonToggleModule,
    MatSlideToggleModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  declarations: [
    CustomerCreateUpdateComponent, 
    LocationTableComponent, 
    ContactTableComponent
  ],
  exports: [
    CustomerCreateUpdateComponent, 
    MatFormFieldModule,
    MatInputModule, 
    MatCheckboxModule
  ]
})
export class CustomerCreateUpdateModule {
}
