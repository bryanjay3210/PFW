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
import { PartsCatalog, User } from 'src/services/interfaces/models';
import { PartsCatalogService } from 'src/services/partscatalog.service';
import { PartsCatalogCreateUpdateComponent } from './parts-catalog-create-update/parts-catalog-create-update.component';

@UntilDestroy()
@Component({
  selector: 'vex-parts-catalog-table',
  templateUrl: './parts-catalog-table.component.html',
  styleUrls: ['./parts-catalog-table.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class PartsCatalogTableComponent implements OnInit {
  @Input() productId: number;
  @Input() partNumber: string;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  imageDefault = "assets/img/pfw_logo_sm.png";

  subject$: ReplaySubject<PartsCatalog[]> = new ReplaySubject<PartsCatalog[]>(1);
  data$: Observable<PartsCatalog[]> = this.subject$.asObservable();
  partsCatalogs: PartsCatalog[];

  dataSource: MatTableDataSource<PartsCatalog> | null;
  selection = new SelectionModel<PartsCatalog>(true, []);
  searchCtrl = new UntypedFormControl()

  pageSizeOptions: number[] = [5, 10, 20, 50];
  pageSize = 10;

  form: UntypedFormGroup;
  mode: 'create' | 'update' = 'create';

  columns: TableColumn<PartsCatalog>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Part Number', property: 'partNumber', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Make', property: 'make', type: 'text', visible: true },
    { label: 'Model', property: 'model', type: 'text', visible: true },
    { label: 'Year From', property: 'yearFrom', type: 'text', visible: true },
    { label: 'Year To', property: 'yearTo', type: 'text', visible: true },
    { label: 'Cylinder', property: 'cylinder', type: 'text', visible: true },
    { label: 'Liter', property: 'liter', type: 'text', visible: true },
    { label: 'Brand', property: 'brand', type: 'text', visible: true },
    { label: 'Notes', property: 'notes', type: 'text', visible: true },
    { label: 'Position', property: 'position', type: 'text', visible: false },
    { label: 'Group Head', property: 'groupHead', type: 'text', visible: false },
    { label: 'Sub Group', property: 'subGroup', type: 'text', visible: false },
    { label: 'Sub Model', property: 'subModel', type: 'text', visible: false },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];

  currentUser = {} as User;
  userPermission = UserPermission;
  access = UserPermission.NoAccess;
  
  constructor(@Inject(MAT_DIALOG_DATA) public defaults: PartsCatalog,
    private dialogRef: MatDialogRef<PartsCatalogCreateUpdateComponent>,
    private fb: UntypedFormBuilder,
    private dialog: MatDialog,
    private route: ActivatedRoute,
    private partsCatalogService: PartsCatalogService,
    private alertService: AlertService,
    private changeDetector: ChangeDetectorRef) {
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
        filter<PartsCatalog[]>(Boolean)
      ).subscribe(partsCatalogs => {
        this.partsCatalogs = partsCatalogs;
        this.dataSource.data = partsCatalogs;
      });
    } else {
      this.defaults = {} as PartsCatalog;
    }

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }

  getData() {
    this.partsCatalogService.getPartsCatalogsByProductId(this.productId).subscribe((result: PartsCatalog[]) => (this.subject$.next(result)));
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

  createPartsCatalog() {
    this.dialog.open(PartsCatalogCreateUpdateComponent, {
      data: {productId: this.productId, partNumber: this.partNumber}
    }).afterClosed().subscribe((partsCatalog: PartsCatalog) => {
      if (partsCatalog) {
        this.partsCatalogService.createPartsCatalog(partsCatalog).subscribe((result: PartsCatalog[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Parts Catalog", "Create");
          }
          else this.alertService.failNotification("Parts Catalog", "Create");
        });
      }
    });
  }

  updatePartsCatalog(partsCatalog: any) {
    this.dialog.open(PartsCatalogCreateUpdateComponent, {
      data: {data: partsCatalog}
    }).afterClosed().subscribe(updatedPartsCatalog => {
      if (updatedPartsCatalog) {
        this.partsCatalogService.updatePartsCatalog(updatedPartsCatalog).subscribe((result: PartsCatalog[]) => {
          if (result) {
            (this.subject$.next(result));
            this.alertService.successNotification("Parts Catalog", "Update");
          }
          else this.alertService.failNotification("Parts Catalog", "Update");
        });
      }
    });
  }

  deletePartsCatalog(partsCatalog: any) { }
  deletePartsCatalogs(partsCatalog: any) { }

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
