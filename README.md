🦁 Simulación de Agentes Racionales: León vs. Impala
Materia: Sistemas Inteligentes (Grupo 1754)

Institución: UNAM - FES Acatlán

Motor: Unity 6 (C#)

Fecha: Diciembre 2025


📋 Descripción del Proyecto
Este proyecto implementa una simulación de Aprendizaje por Refuerzo (Reinforcement Learning) donde un agente depredador (León) debe aprender autónomamente a cazar a una presa (Impala) en un entorno estocástico y discreto.

El sistema utiliza algoritmos de Q-Learning Simplificado, Memoria Retrospectiva y estrategias Epsilon-Greedy para evolucionar de un comportamiento aleatorio a una estrategia de caza planificada, respetando reglas de visión, sigilo y movimiento por turnos.


🚀 Guía de Instalación y Ejecución (Paso a Paso)
Este software no requiere instalación formal, se ejecuta como una aplicación portable ("Standalone").

1. Descarga y Preparación
Descargue el archivo comprimido Juego Final SI.rar que esta dentro de la carpeta Entrega_Ejecutable desde este repositorio.
rar
Mueva el archivo a una ubicación segura (ej. Escritorio o Documentos).

2. Descompresión (¡IMPORTANTE!)
⚠️ No ejecute el juego directamente desde el archivo rar.

Haga clic derecho sobre el archivo .rar

Seleccione "Extraer todo..." o "Extraer aquí".

Asegúrese de que la carpeta extraída contenga tanto el archivo .exe como las carpetas de datos (_Data, MonoBleedingEdge).

3. Ejecución
Abra la carpeta descomprimida.

Haga doble clic en SimulacionLeon.exe (el icono del León/Unity).

Nota: Si Windows muestra una advertencia de seguridad (SmartScreen), haga clic en "Más información" y luego en "Ejecutar de todas formas".

4. Controles dentro de la Simulación
Modo Entrenamiento: Acelera la simulación (x50 velocidad) para entrenar al agente rápidamente.

Reiniciar Juego: Comienza una nueva sesión visual manteniendo el conocimiento adquirido.

Salir: Cierra la aplicación.

🧠 Base de Conocimientos (Persistencia)
El "cerebro" del agente no está programado con reglas If-Else, sino que es una estructura de datos generada empíricamente.

Estructura de Datos
El conocimiento se almacena en pares de Estado-Acción-Valor:

Estado: Posición_X + AcciónImpala (ej. Pos3_MirandoFrente).

Acción: Avanzar o Esconderse.

Valor Q: Puntaje acumulado que indica la "calidad" de esa decisión.

Ubicación del Archivo
El conocimiento se guarda automáticamente en un archivo JSON en la ruta persistente del usuario:

C:\Users\[TU_USUARIO]\AppData\LocalLow\DefaultCompany\SimulacionLeonImpala\cerebro_leon.json

Cómo Reiniciar el Aprendizaje (Hard Reset)
Para que el León olvide todo y empiece desde cero (Tabula Rasa):

Cierre la aplicación.

Vaya a la ruta mencionada arriba.

Elimine el archivo cerebro_leon.json.

Al abrir el juego de nuevo, el agente comenzará en "Fase 0" (Aleatoria).

🎓 Proceso de Entrenamiento y Adquisición
El agente atraviesa tres fases evolutivas durante la simulación:

1. Fase de Exploración Pura (Novato)
Comportamiento: El agente elige acciones al azar (50/50).

Resultado: Tasa de victorias baja (<15%). Muere frecuentemente por entrar en el campo de visión.

2. Fase de Explotación / Trauma (Intermedio)
Adquisición: El agente aprende que "Avanzar" conlleva riesgo de muerte (-100 pts) y "Esconderse" es seguro (0 pts).

Comportamiento: Tiende a quedarse escondido excesivamente para evitar castigos.

3. Fase Experta (Curiosidad y Planificación)Para superar el estancamiento, el sistema implementa:Curiosidad ($\epsilon$-Greedy): En el 10% de los turnos, el agente ignora su miedo y prueba acciones arriesgadas para descubrir nuevas rutas.Aprendizaje Retrospectivo: Al lograr una cacería exitosa (+1000 pts), el sistema recompensa toda la cadena de movimientos anteriores, no solo el último paso.Resultado: El agente aprende a flanquear, esconderse solo cuando es necesario y atacar en el momento preciso.

⚙️ Reglas del Sistema (Resumen Técnico)
El simulador valida estrictamente las reglas del documento de requerimientos :

Turnos Discretos: El Impala actúa y el León reacciona en el mismo turno T.
Visión del Impala: Simulación física mediante Raycasting y ángulo de visión de 45°.
Sigilo: El estado "Escondido" anula la detección visual del Impala.
Distancia Crítica: Si la distancia es < 3 cuadros, el Impala huye por instinto (incluso si el león está escondido).


👥 Créditos
Desarrollado por:

Casas Morales, Manuel de Jesús

Ricaño González, Daniel

Rodríguez Arteaga, Jafet

Rodríguez Pérez, Gerardo

Sistemas Inteligentes 2026-1
