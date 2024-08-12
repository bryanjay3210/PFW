import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { VexModule } from '../@vex/vex.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CustomLayoutModule } from './custom-layout/custom-layout.module';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { PopUpModule } from './pages/apps/components/pop-up/pop-up.module';
import { TokenInterceptorService } from 'src/services/token-interceptor/token-interceptor.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { BnNgIdleService } from 'bn-ng-idle';
import * as $ from 'jquery';
//import { PrintLayoutComponent } from './print-layout/print-layout.component';
//import { InvoiceComponent } from './invoice/invoice.component';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MatDialogModule,
    MatIconModule,
    PopUpModule,
    MatFormFieldModule,
    MatInputModule,
    NgMultiSelectDropDownModule.forRoot(),
    
    // Vex
    VexModule,
    CustomLayoutModule
  ],
  providers: [{provide: HTTP_INTERCEPTORS, useClass: TokenInterceptorService, multi: true}, BnNgIdleService],
  bootstrap: [AppComponent]
})
export class AppModule { }
