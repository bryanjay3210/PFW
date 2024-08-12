import { AfterViewInit, ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { Notification } from '../interfaces/notification.interface';
import { DateTime } from 'luxon';
import { trackById } from '../../../../utils/track-by';
import { CustomerNote, OrderNote, User} from 'src/services/interfaces/models';
import { UserService } from 'src/services/user.service';
import moment from 'moment';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ToolbarNotificationsCustomerNoteDialog } from '../toolbar-notifications-customer-note-dialog/toolbar-notifications-customer-note-dialog';
import { Router } from '@angular/router';
import { range } from 'rxjs';
import { SharedService } from '../services/shared.service';
import { AlertService } from 'src/services/alert.service';
import { CustomerNoteService } from 'src/services/customernote.service';
import { OrderNoteService } from 'src/services/ordernote.service';

@Component({
  selector: 'vex-toolbar-notifications-dropdown',
  templateUrl: './toolbar-notifications-dropdown.component.html',
  styleUrls: ['./toolbar-notifications-dropdown.component.scss']
})
export class ToolbarNotificationsDropdownComponent implements OnInit, AfterViewInit {
  currentUser = {} as User;
  followUpNotifications: Notification[] = []
  creditNoteNotifications: Notification[] = []
  totalNotifications: number = 0;
  trackById = trackById;
  

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private userService: UserService,
    private sharedService: SharedService,
    private alertService: AlertService,
    private customerNoteService: CustomerNoteService,
    private orderNoteService: OrderNoteService
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
  }
  
  ngAfterViewInit(): void {
    //throw new Error('Method not implemented.');
  }

  ngOnInit() {
    this.sharedService
      .updateCountTriggered$()
      .subscribe(() => this.getUserNotifications());

    this.getUserNotifications();
  }

  private getUserNotifications() {
    this.userService.getUserNotificationsByUserId(this.currentUser.id).subscribe(result => {
      if (result) {
        this.followUpNotifications = [];
        result.followUpList.forEach(e => {
          const notification = {} as Notification;
          notification.id = e.customerId.toString();
          notification.colorClass = '';
          notification.icon = 'mat:account_circle';
          notification.label = e.customerName;
          notification.phone = e.phoneNumber;
          notification.email = e.email;
          this.followUpNotifications.push(notification);
        });

        this.creditNoteNotifications = [];
        result.creditNoteList.forEach(e => {
          const notification = {} as Notification;
          notification.id = e.customerId.toString();
          notification.colorClass = '';
          notification.icon = 'mat:monetization_on';
          notification.label = e.customerName;
          notification.phone = e.phoneNumber;
          notification.email = e.email;
          notification.amount = e.amount;
          notification.creditMemoNumber = e.creditMemoNumber;
          notification.orderId = e.orderId;
          this.creditNoteNotifications.push(notification);
        });

        this.totalNotifications = this.followUpNotifications.length + this.creditNoteNotifications.length;

        this.cd.detectChanges;
      }
    });
  }

  formatCurrency(amount: number) {
    return (amount) ? amount.toFixed(2) : '0.00';
  }

  addCustomerNote(row: Notification) {
    if (row.note.trim().length === 0) return;

    this.alertService.createNotification('Customer Note').then(answer => {
      if (!answer.isConfirmed) return;
      const customerNote = {} as CustomerNote;
      customerNote.customerId = Number(row.id);
      customerNote.createdBy = this.currentUser.userName;
      customerNote.createdDate = moment(new Date());
      customerNote.notes = row.note.trim();
      customerNote.isActive = true
      customerNote.isDeleted = false;
      this.customerNoteService.createCustomerNote(customerNote).subscribe({
        next: (result) => {
          if (result) {
            this.getUserNotifications();
            this.sharedService.updateCountTrigger();
            this.cd.detectChanges();
          }
        },
        error: (e) => {
          this.alertService.failNotification("Customer Notes", "Create");
          console.error(e)
        },
        complete: () => console.info('complete') 
      });
    });

    // this.dialog.open(ToolbarNotificationsCustomerNoteDialog, {
    //   height: '60%',
    //   width: '60%',
    //   data: row
    // }).afterClosed().subscribe(r => {
    //   this.getUserNotifications();
    // });
  }

  addCreditNote(row: Notification) {
    if (row.note.trim().length === 0) return;

    this.alertService.createNotification('Credit Note').then(answer => {
      if (!answer.isConfirmed) return;
      const orderNote = {} as OrderNote;
      orderNote.orderId = row.orderId;
      orderNote.createdBy = this.currentUser.userName;
      orderNote.createdDate = moment(new Date());
      orderNote.notes = row.note.trim();
      orderNote.isActive = true
      orderNote.isDeleted = false;
      this.orderNoteService.createOrderNote(orderNote).subscribe({
        next: (result) => {
          if (result) {
            this.getUserNotifications();
            this.sharedService.updateCountTrigger();
            this.cd.detectChanges();
          }
        },
        error: (e) => {
          this.alertService.failNotification("Credit Note", "Create");
          console.error(e)
        },
        complete: () => console.info('complete') 
      });
    });
  }

  getOrderNumber(notification : Notification) {
    return { 'state' : { 'orderNumber' : notification.creditMemoNumber }};
  }

  addCreditNote_Orig(notification : Notification) {
    this.router.navigate(['/apps/administration/order-management'], { state: { orderNumber: notification.creditMemoNumber }});
    //this.sharedService.changeMessage(notification.creditMemoNumber.toString());
  }
}
