import { Component, OnInit } from '@angular/core';
import { PersonService, Person } from './person.service';

@Component({
    selector: 'my-app',
    templateUrl: './html/app.componment.html',
    providers: [
        PersonService
    ]
})
export class AppComponent extends OnInit {

    constructor(private _service: PersonService) {
        super();
    }

    ngOnInit() {
        this._service.loadData().then(data => {
            this.persons = data;
        })
    }

    persons: Person[] = [];
}