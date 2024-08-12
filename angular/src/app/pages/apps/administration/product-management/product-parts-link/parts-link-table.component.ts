import { SelectionModel } from '@angular/cdk/collections';
import { ChangeDetectorRef, Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { filter } from 'rxjs/operators';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { ReplaySubject } from 'rxjs/internal/ReplaySubject';
import { TableColumn } from 'src/@vex/interfaces/table-column.interface';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { AlertService } from 'src/services/alert.service';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { ModuleCode, UserPermission } from 'src/static-data/enums/enums';
import { ItemMasterlistReference, User } from 'src/services/interfaces/models';
import { ItemMasterlistReferenceService } from 'src/services/itemmasterlistreference.service';
import { PartsLinkCreateUpdateComponent } from './parts-link-create-update/parts-link-create-update.component';

@UntilDestroy()
@Component({
  selector: 'vex-parts-link-table',
  templateUrl: './parts-link-table.component.html',
  styleUrls: ['./parts-link-table.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class PartsLinkTableComponent implements OnInit {
  @Input() productId: number;
  @Input() partNumber: string;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  imageDefault = "assets/img/pfw_logo_sm.png";

  subject$: ReplaySubject<ItemMasterlistReference[]> = new ReplaySubject<ItemMasterlistReference[]>(1);
  data$: Observable<ItemMasterlistReference[]> = this.subject$.asObservable();
  partsLinks: ItemMasterlistReference[];

  dataSource: MatTableDataSource<ItemMasterlistReference> | null;
  selection = new SelectionModel<ItemMasterlistReference>(true, []);
  searchCtrl = new UntypedFormControl()

  pageSizeOptions: number[] = [5, 10, 20, 50];
  pageSize = 10;
  
  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  columns: TableColumn<ItemMasterlistReference>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Part Number', property: 'partNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Parts Link Number', property: 'partsLinkNumber', type: 'text', visible: true },
    { label: 'OEM Number', property: 'oemNumber', type: 'text', visible: true },
    { label: 'Main Parts Link', property: 'isMainPartsLink', type: 'text', visible: true },
    { label: 'Main OEM', property: 'isMainOEM', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  
  constructor(@Inject(MAT_DIALOG_DATA) public defaults: ItemMasterlistReference,
    private dialog: MatDialog,
    private itemMasterlistReferenceService: ItemMasterlistReferenceService,
    private alertService: AlertService,
    private cd: ChangeDetectorRef) {
    this.currentUser = JSON.parse(localStorage.getItem('CurrentUser'));
    let modulePermission = this.currentUser.role.rolePermissions.find(rp => rp.module.code == ModuleCode.ProductManagement);
    this.access = modulePermission.accessTypeId;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnInit(): void {
    this.getData();

    if (this.defaults) {
      this.mode = 'update';
      this.dataSource = new MatTableDataSource();
      this.data$.pipe(
        filter<ItemMasterlistReference[]>(Boolean)
      ).subscribe(partsLinks => {
        this.partsLinks = partsLinks;
        this.dataSource.data = partsLinks;
      });
    } else {
      this.defaults = {} as ItemMasterlistReference;
    }

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  getData() {
    this.itemMasterlistReferenceService.getItemMasterlistReferencesByProductId(this.productId).subscribe((result: ItemMasterlistReference[]) => (this.subject$.next(result)));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.dataSource.filter = value;
  }

  ngAfterViewInit() {
    if (this.dataSource) {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    }
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  createPartsLink() {
    this.dialog.open(PartsLinkCreateUpdateComponent, {
      data: {productId: this.productId, partNumber: this.partNumber}
    }).afterClosed().subscribe((itemMasterlistReference: ItemMasterlistReference) => {
      if (itemMasterlistReference) {
        this.itemMasterlistReferenceService.createItemMasterlistReferenceByProduct(itemMasterlistReference).subscribe((result: ItemMasterlistReference[]) => {
          if (result) {
            this.alertService.successNotification("Parts Link & OEM", "Create");
            (this.subject$.next(result));
          }
          else this.alertService.failNotification("Parts Link & OEM", "Create");
        });
      }
    });
  }

  updatePartsLink(itemMasterlistReference: any) {
    this.dialog.open(PartsLinkCreateUpdateComponent, {
      data: {data: itemMasterlistReference}
    }).afterClosed().subscribe(updatedItemMasterlistReference => {
      if (updatedItemMasterlistReference) {
        this.itemMasterlistReferenceService.updateItemMasterlistReferenceByProduct(updatedItemMasterlistReference).subscribe((result: ItemMasterlistReference[]) => {
          if (result) {
            this.alertService.successNotification("Parts Link & OEM", "Update");
            (this.subject$.next(result));
          }
          else this.alertService.failNotification("Parts Link & OEM", "Update");
        });
      }
    });
  }

  deletePartsLink(itemMasterlistReference: any) { }
  deletePartsLinks(itemMasterlistReference: any) { }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
}
