import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-barra-busca',
  standalone: true,
  imports: [FormsModule, ButtonModule, InputTextModule],
  templateUrl: './barra-busca.component.html',
  styleUrl: './barra-busca.component.css'
})
export class BarraBuscaComponent {
  termo = '';

  @Input() desabilitado = false;

  @Output() buscar = new EventEmitter<string>();

  pesquisar(): void {
    const termo = this.termo.trim();

    if (termo && !this.desabilitado) {
      this.buscar.emit(termo);
    }
  }
}
