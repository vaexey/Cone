import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StartPageComponent } from './start-page/start-page.component';
import { LayoutModule } from '../layout/layout.module';
import { AppModule } from '../app.module';



@NgModule({
  declarations: [
    StartPageComponent
  ],
  imports: [
    CommonModule,
    AppModule,
    LayoutModule
  ]
})
export class PagesModule { }
