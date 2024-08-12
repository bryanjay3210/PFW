import { ChangeDetectionStrategy, ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { PopoverService } from '../../../components/popover/popover.service';
import { ToolbarNotificationsDropdownComponent } from './toolbar-notifications-dropdown/toolbar-notifications-dropdown.component';
import { User } from 'src/services/interfaces/models';
import { Notification } from './interfaces/notification.interface';
import { UserService } from 'src/services/user.service';
import moment from 'moment';
import { SharedService } from './services/shared.service';

@Component({
  selector: 'vex-toolbar-notifications',
  templateUrl: './toolbar-notifications.component.html',
  styleUrls: ['./toolbar-notifications.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ToolbarNotificationsComponent implements OnInit {

  @ViewChild('originRef', { static: true, read: ElementRef }) originRef: ElementRef;

  dropdownOpen: boolean;
  currentUser = {} as User;
  followUpNotifications: Notification[] = []
  creditNoteNotifications: Notification[] = []
  totalNotifications: number = 0;

  constructor(
    private popover: PopoverService,
    private userService: UserService,
    private cd: ChangeDetectorRef,
    private sharedService: SharedService) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
  }

  ngOnInit() {
    this.sharedService
      .updateCountTriggered$()
      .subscribe(() => this.updatePopoverCount());
    this.getUserNotifications();
  }

  private getUserNotifications() {
    // this.followUpNotifications = [];
    this.userService.getUserNotificationsByUserId(this.currentUser.id).subscribe(result => {
      if (result) {
        // result.followUpList.forEach(e => {
        //   const notification = {} as Notification;
        //   notification.id = e.customerId.toString();
        //   notification.colorClass = '';
        //   //notification.datetime = this.formatDate(e.lastContactDate);
        //   notification.icon = 'mat:account_circle';
        //   notification.label = e.customerName;
        //   notification.phone = e.phoneNumber;
        //   notification.email = e.email;
        //   this.followUpNotifications.push(notification);
        // });

        // result.creditNoteList.forEach(e => {
        //   const notification = {} as Notification;
        //   notification.id = e.customerId.toString();
        //   notification.colorClass = '';
        //   //notification.datetime = this.formatDate(e.lastContactDate);
        //   notification.icon = 'mat:account_circle';
        //   notification.label = e.customerName;
        //   notification.phone = e.phoneNumber;
        //   notification.email = e.email;
        //   notification.amount = e.amount;
        //   notification.creditMemoNumber = e.creditMemoNumber;
        //   this.creditNoteNotifications.push(notification);
        // });

        this.totalNotifications = result.followUpList.length + result.creditNoteList.length;
        this.cd.detectChanges();
      }
    });
  }

  showPopover() {
    this.dropdownOpen = true;
    this.cd.markForCheck();

    const popoverRef = this.popover.open({
      content: ToolbarNotificationsDropdownComponent,
      origin: this.originRef,
      offsetY: 12,
      position: [
        {
          originX: 'center',
          originY: 'top',
          overlayX: 'center',
          overlayY: 'bottom'
        },
        {
          originX: 'end',
          originY: 'bottom',
          overlayX: 'end',
          overlayY: 'top',
        },
      ],
    });

    popoverRef.afterClosed$.subscribe(() => {
      this.dropdownOpen = false;
      this.cd.markForCheck();
    });
  }

  updatePopoverCount() {
    this.totalNotifications = 0;
    this.getUserNotifications();
  }
}
