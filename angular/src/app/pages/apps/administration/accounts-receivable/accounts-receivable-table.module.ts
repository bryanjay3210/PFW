import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountsReceivableTableRoutingModule } from './accounts-receivable-table-routing.module';
import { AccountsReceivableTableComponent } from './accounts-receivable-table.component';
import { PageLayoutModule } from 'src/@vex/components/page-layout/page-layout.module';
import { BreadcrumbsModule } from 'src/@vex/components/breadcrumbs/breadcrumbs.module';
import { AccountsReceivableCreateUpdateModule } from './accounts-receivable-create-update/accounts-receivable-create-update.module';
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


@NgModule({
  declarations: [AccountsReceivableTableComponent],
  imports: [
    CommonModule,
    AccountsReceivableTableRoutingModule,
    PageLayoutModule,
    BreadcrumbsModule,
    AccountsReceivableCreateUpdateModule,
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
    MatButtonToggleModule
  ]
})
export class AccountsReceivableTableModule {
}
