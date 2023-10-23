import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RoutesRecognized } from '@angular/router';
import { filter, map, Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements
  OnInit {
  title = 'Cone Panel';

  public windowTitle:string = "";

  constructor(
    private router: Router
  )
  {}

  ngOnInit(): void
  {
    this.router.events
      .pipe(
        filter((ev) => ev instanceof NavigationEnd),
        map(() => {
          let r: ActivatedRoute = this.router.routerState.root;
          let t = "";

          while(r.firstChild != null)
            r = r.firstChild;
          
          if(r.snapshot.data['windowTitle'])
            t = r.snapshot.data['windowTitle'];

          return t;
        })
      )
      .subscribe((title: string) => {
        this.windowTitle = title;
      })
  }
}
