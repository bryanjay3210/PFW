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
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { HttpClientModule } from '@angular/common/http';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDialogModule } from '@angular/material/dialog';
import { MatRadioModule } from '@angular/material/radio';
import { MatDividerModule } from '@angular/material/divider';
import { RouterModule } from '@angular/router';
import { QuicklinkModule } from 'ngx-quicklink';
import { SecondaryToolbarModule } from 'src/@vex/components/secondary-toolbar/secondary-toolbar.module';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { ReportCustomerListModule } from '../../report-management/report-customer-list/report-customer-list.module';
import { EmailModuleComponent } from './email-module.component';
import { EmailModuleRoutingModule } from './email-module-routing.module';
import { MatSidenavModule } from '@angular/material/sidenav';
import { ScrollbarModule } from 'src/@vex/components/scrollbar/scrollbar.module';
import { RelativeDateTimeModule } from 'src/@vex/pipes/relative-date-time/relative-date-time.module';
import { StripHtmlModule } from 'src/@vex/pipes/strip-html/strip-html.module';
import { QuillModule } from 'ngx-quill';
// import { MailRoutingModule } from '../../mail/mail-routing.module';
// import { MailRoutingModule } from '../../mail/mail-routing.module';

@NgModule({
  declarations: [EmailModuleComponent],
  imports: [
    CommonModule,
    MatExpansionModule,
    MatButtonModule,
    MatIconModule,
    EmailModuleRoutingModule,
    ReportCustomerListModule,
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
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule,
    MatDatepickerModule,
    MatNativeDateModule,
    HttpClientModule,
    MatTabsModule,
    MatDialogModule,
    MatRadioModule,
    MatDividerModule,
    RouterModule,
    QuicklinkModule,
    SecondaryToolbarModule,
    MatAutocompleteModule,
    MatNativeDateModule, 
    MatPaginatorModule,
    MatTableModule,
    MatSortModule,
    MatButtonToggleModule,
    MatSlideToggleModule,
    MatFormFieldModule,
    MatSelectModule,
    MatRadioModule,

    MatSidenavModule,
    ScrollbarModule,
    MatRippleModule,
    RelativeDateTimeModule,
    StripHtmlModule,

    // MailRoutingModule,
    QuillModule.forRoot({
      modules: {
        toolbar: [
          ['bold', 'italic', 'underline', 'strike'],
          ['blockquote', 'code-block'],

          [{ list: 'ordered' }, { list: 'bullet' }],

          [{ header: [1, 2, 3, 4, 5, 6, false] }],

          ['clean'],

          ['link', 'image']
        ]
      }
    }),
  ]
})
export class EmailModuleModule {
}
