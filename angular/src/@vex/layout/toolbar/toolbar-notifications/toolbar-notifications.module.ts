import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToolbarNotificationsComponent } from './toolbar-notifications.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PopoverModule } from '../../../components/popover/popover.module';
import { MatTabsModule } from '@angular/material/tabs';
import { MatMenuModule } from '@angular/material/menu';
import { RelativeDateTimeModule } from '../../../pipes/relative-date-time/relative-date-time.module';
import { RouterModule } from '@angular/router';
import { MatRippleModule } from '@angular/material/core';
import { ToolbarNotificationsDropdownComponent } from './toolbar-notifications-dropdown/toolbar-notifications-dropdown.component';
import { ToolbarNotificationsCustomerNoteDialogModule } from './toolbar-notifications-customer-note-dialog/toolbar-notifications-customer-note-dialog.module';
import { SharedService } from './services/shared.service';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';


@NgModule({
  providers: [SharedService],
  declarations: [ToolbarNotificationsComponent, ToolbarNotificationsDropdownComponent],
  imports: [
    BrowserModule,
    FormsModule,
    CommonModule,
    MatButtonModule,
    MatIconModule,
    PopoverModule,
    MatTabsModule,
    MatMenuModule,
    RelativeDateTimeModule,
    RouterModule,
    MatRippleModule,
    ToolbarNotificationsCustomerNoteDialogModule
  ],
  exports: [ToolbarNotificationsComponent]
})
export class ToolbarNotificationsModule {
}
