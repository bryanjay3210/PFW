import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { QuicklinkModule } from 'ngx-quicklink';
import { VexRoutes } from 'src/@vex/interfaces/vex-route.interface';
import { EmailModuleComponent } from './email-module.component';


const routes: VexRoutes = [
  {
    path: '',
    component: EmailModuleComponent,
    data: {
      toolbarShadowEnabled: false
    }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule, QuicklinkModule]
})
export class EmailModuleRoutingModule {
}
