import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QuicklinkModule } from 'ngx-quicklink';
import { VexRoutes } from 'src/@vex/interfaces/vex-route.interface';
import { PurchaseOrderTableComponent } from './purchase-order-table.component';


const routes: VexRoutes = [
  {
    path: '',
    component: PurchaseOrderTableComponent,
    data: {
      toolbarShadowEnabled: false
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule, QuicklinkModule]
})
export class PurchaseOrderTableRoutingModule {
}
