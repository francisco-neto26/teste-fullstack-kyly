import { Component, Input } from '@angular/core';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

// Componente de carregamento com spinner
@Component({
  selector: 'app-carregando',
  standalone: true,
  imports: [ProgressSpinnerModule],
  templateUrl: './carregando.component.html',
  styleUrl: './carregando.component.css'
})
export class CarregandoComponent {
  @Input() mensagem = 'Carregando resultados...';
}
