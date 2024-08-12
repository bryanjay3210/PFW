import { AfterViewInit, Component, Inject, OnInit } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UntilDestroy } from '@ngneat/until-destroy';
import moment from 'moment';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { AlertService } from 'src/services/alert.service';
import { CustomerNote, User } from 'src/services/interfaces/models';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { MatTableDataSource } from '@angular/material/table';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { CustomerNoteService } from 'src/services/customernote.service';

@UntilDestroy()
@Component({
  selector: 'vex-toolbar-notifications-customer-note-dialog',
  templateUrl: './toolbar-notifications-customer-note-dialog.html',
  styleUrls: ['./toolbar-notifications-customer-note-dialog.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class ToolbarNotificationsCustomerNoteDialog implements OnInit, AfterViewInit {
  columns: TableColumn<CustomerNote>[] = [
    { label: 'Date', property: 'createdDate', type: 'text', visible: true },
    { label: 'Note', property: 'notes', type: 'text', visible: true },
    { label: 'Created By', property: 'createdBy', type: 'text', visible: true },
  ];
  
  dataSource: MatTableDataSource<CustomerNote> | null;
  customerNoteCtrl = new UntypedFormControl;
  sortColumn: string = '';
  sortOrder: string = '';
  
  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  customerId: number = 0;
  customerName: string = '';
  customerPhone: string = '';
  customerEmail: string = '';

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
    private dialogRef: MatDialogRef<ToolbarNotificationsCustomerNoteDialog>,
    private customerNoteService: CustomerNoteService,
    private alertService: AlertService
  ) {

    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.OrderManagement);
    this.access = modulePermission.accessTypeId;
  }

  ngOnInit(): void {
    this.dataSource = new MatTableDataSource();

    if (this.defaults.id) {
      this.customerId = this.defaults.id;
      this.customerName = this.defaults.label;
      this.customerPhone = this.defaults.phone;
      this.customerEmail = this.defaults.email;
    }
    else {
      this.customerId = this.defaults.customerId;
      this.customerName = this.defaults.customerName;
      this.customerPhone = this.defaults.phoneNumber;
      this.customerEmail = this.defaults.email;
    }

    this.getCustomerNotes();
  }

  ngAfterViewInit() {
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  sortDataMatTable(event) {
    this.sortColumn = event.active;
    this.sortOrder = event.direction;
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  getCustomerNotes() {
    this.customerNoteService.getCustomerNotesByCustomerId(this.customerId).subscribe({
      next: (result) => {
        if (result) {
          this.dataSource.data = result;
        }
      },
      error: (e) => {
        this.alertService.failNotification("Customer Notes", "Fetch");
        console.error(e)
      },
      complete: () => console.info('complete') 
    });
  }

  createNote() {
    if (this.customerNoteCtrl.value.trim().length === 0) return;
    this.alertService.createNotification('Customer Note').then(answer => {
      if (!answer.isConfirmed) return;
      const customerNote = {} as CustomerNote;
      customerNote.customerId = this.customerId;
      customerNote.createdBy = this.currentUser.userName;
      customerNote.createdDate = moment(new Date());
      customerNote.notes = this.customerNoteCtrl.value.trim();
      customerNote.isActive = true
      customerNote.isDeleted = false;
      this.customerNoteService.createCustomerNote(customerNote).subscribe({
        next: (result) => {
          if (result) {
            this.dataSource.data = result;
            this.customerNoteCtrl.setValue('');
            if (!this.defaults.id) {
              this.defaults.lastUpdateDate = customerNote.createdDate;
            }
          }
        },
        error: (e) => {
          this.alertService.failNotification("Customer Notes", "Create");
          console.error(e)
        },
        complete: () => console.info('complete') 
      });
    });
  }

  formatDate(orderDate: moment.Moment) {
    return moment(orderDate).format('MM/DD/YYYY h:mm A');
  }
}