import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-estado-vazio',
  standalone: true,
  imports: [],
  templateUrl: './estado-vazio.component.html',
  styleUrl: './estado-vazio.component.css'
})
export class EstadoVazioComponent {
  @Input() icone = 'pi pi-search';
  @Input() titulo = 'Nenhum resultado';
  @Input() descricao = 'Tente buscar com outro termo.';
}
