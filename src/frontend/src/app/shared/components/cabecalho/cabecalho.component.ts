import { Component, EventEmitter, Output, inject } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { AvatarModule } from 'primeng/avatar';
import { TagModule } from 'primeng/tag';
import { AuthService } from '../../../core/services/auth.service';
// Componente de cabeçalho/navbar da tela principal, exibindo o nome do usuário e botão de logout
@Component({
  selector: 'app-cabecalho',
  standalone: true,
  imports: [ButtonModule, AvatarModule, TagModule],
  templateUrl: './cabecalho.component.html',
  styleUrl: './cabecalho.component.css'
})
export class CabecalhoComponent {
  @Output() sair = new EventEmitter<void>();

  private readonly authService = inject(AuthService);

  get usuarioNome(): string {
    return this.authService.usuario;
  }
}
