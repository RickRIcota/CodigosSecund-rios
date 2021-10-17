//* Funções Return => VOID

    //print(n,msg)
    void print(int n, string msg){bc.printLCD(n, msg);}

    void printClear(){bc.ClearConsole();}

    //delay(n)
    void delay(int n){bc.wait(n);}

    //motor(left,right)
    void motor(int left, int right){bc.onTF(left, right);}

    //rot(speed,angle)
    void rot(int speed, float angle){bc.onTFRot(speed, angle);}

    //led("cor do led")
    void led(string color){bc.turnLedOn(color);}

    //led("cor do led")
    void ledRGB(int a, int b, int c){bc.turnLedOn(a, b, c);}

    //ledOff()
    void ledOff(){bc.turnLedOff();}

    //rstTimer()
    void rstTimer(){bc.resetTimer();}

    //descerGarra(speed, tempo) //velocidade max = 150
    void descerGarra(int speed, int time){bc.ActuatorSpeed(speed); bc.ActuatorDown(time);}
        
    //descerGarra(speed, tempo)
    void subirGarra(int speed, int time) {bc.ActuatorSpeed(speed); bc.ActuatorUp(time);}
        
    //descerGarra(speed, tempo)
    void turnUp(int time){bc.TurnActuatorUp(time);}

    //descerGarra(speed, tempo)
    void turnDown(int time){bc.TurnActuatorDown(time);}
    //SUBIR: 160 - 132 DESCER: 78 - 40   ATUATOR: 160 - 78   SCOOP: 132 - 40



//* Funções Return => VALUE

    //cor(sensor)
    string cor(int sensor){return bc.returnColor(sensor - 1);}

    //luz(sensor)
    float luz(int sensor){return bc.lightness(sensor - 1);}

    //corB(sensor)
    float corB(int sensor){return bc.returnBlue(sensor - 1);}

    //corG(sensor)
    float corG(int sensor){return bc.returnGreen(sensor - 1);}

    //corR(sensor)
    float corR(int sensor){return bc.returnRed(sensor - 1);}

    //compass()
    float compass(){return bc.Compass();}

    //sonar(sensor)
    float sonar(int sensor){return bc.distance(sensor);}
        //sonar(0)  => CIMA
        //sonar(1)  => MEIO
        //sonar(2)  => BAIXO

    //inclinação
    float inclina(){return bc.Inclination();}

    //Timer()
    int timer(){return bc.timer();}



//* Variáveis
int velocidade = 160; 
int Sensibilidade = 25; //padrão 25
int detecRampa = 0;
int numeroBolinhas = 0;
int numeroBolinhasPretas = 0;
int numeroBolinhasBrancas = 0;
int distancia = 120;
int g = 0;
int n = 0;

int tempo = 0;

float pos1 = 0;
float pos2 = 0;
float pos3 = 0;

float linha = 40; //MUDAR CONFORME HORARIO 
float branco = 70;
float angulo;
float anguloParaVoltar;
float anguloDeVolta = 0;
float anguloInicial = 0;
float distanciaAtéBolinha = 0;
double tempoDeIda = 0;

float tempoDeGap = 0;

bool doisVerdes = false;
bool semCirculo = false;

bool resgatePos1 = false;
bool resgatePos2 = false;
bool resgatePos3 = false;

bool saidaPos1 = false;
bool saidaPos2 = false;
bool saidaPos3 = false;

bool verifClassif = false;
bool pegarBolinha = false;
bool giro = false;
bool resgateDeRe = false;
bool leftturn = false;
bool vermelhoSaida = false;



//* Função Principal
void Main(){
    
    //!setup

        //detectar rampa
        if(sonar(1) <= 50){
            
            if(bc.AngleActuator() >= 287){
                Descer(147, 130);
            }else{
                //subir garra no inicio
                Subir(147, 130);
            }

            print(1,"Rampa = True");
            led("amarelo");

            InitResgate();
        }else{
            //subir garra no inicio
            Subir(160, 130);
        }
        bc.colorSens(Sensibilidade);

        //Teste();

    //!loop 
    while(true){	 

        Verde2();
        LineFollow();
        Obstaculo();
        VerifRampa();
        VerifSuperGap(2300);
        //Gangorra();
    }
}

void Teste(){
    
    //COLOCAR CODIGO TESTE
    


    //!////////////////////////
    motor(0,0);
    delay(1000000);
}



//* Funções Secundárias
float CorreçãoAngulo(int l){
    
    //ATUADOR
    if(l == 1){

        if(bc.AngleActuator() >= 270 && bc.AngleActuator() <= 360){
            angulo = (360 - bc.AngleActuator()) + 90;
        }
        if(bc.AngleActuator() >= 0 && bc.AngleActuator() <= 89){
            angulo = 90 - bc.AngleActuator();
        } 
        else if (bc.AngleActuator() == 0){
            angulo = bc.AngleActuator() + 90;
        }
        
    }

    //SCOOP
    if(l == 2){

        if(bc.AngleScoop() >= 270 && bc.AngleScoop() <= 360){
            angulo = (360 - bc.AngleScoop()) + 90;
        }  
        if(bc.AngleScoop() >= 0 && bc.AngleScoop() <= 89){
            angulo = 90 - bc.AngleScoop();
        } 
        else if (bc.AngleScoop() == 0){
            angulo = bc.AngleScoop() + 90;
        }
        
    }
    return angulo;  
}

void Descer(int a, int b){
    while(CorreçãoAngulo(1) > a){   
        descerGarra(150, 1);
    }
    while(CorreçãoAngulo(2) > b){   
        turnUp(1);
    }
}

void Subir(int a, int b){
    while(CorreçãoAngulo(1) < a){   
        subirGarra(120, 1);
    }
    while(CorreçãoAngulo(2) < b){   
        turnDown(1);
    }
}



//* Funções para seguir linha
void LineFollow(){
    
    saida:
    
    //Frente
    motor(velocidade,velocidade);
    //vermelho da Saida 
    if(vermelhoSaida && cor(2) == "VERMELHO" || cor(3) == "VERMELHO" || cor(4) == "VERMELHO"){
        motor(300,300);
        delay(500);
        ledRGB(250,80,100);
        motor(0,0);
        delay(10000);
    }

    //vermelho fora da pista
    if(!vermelhoSaida && ((corB(1) < corR(1) && corG(1) < corR(1) && corR(1) > 30) || (corB(5) < corR(5) && corG(5) < corR(5) && corR(5) > 30)) ){
        ledRGB(200,80,80);
        print(0, "Sai da Pista, voltando...");
        
        while(cor(3) != "PRETO"){
            motor(-150,-150);
        }

        ledRGB(0,200,100);

        motor(-200,-200);
        delay(200);

        rot(1000, -10);

        printClear();
        ledOff();
    }
    
    //Virar para Esquerda
    if((luz(5) < linha || luz(4) < linha) && (luz(1) > linha) && (cor(1) != "VERMELHO" || cor(5) != "VERMELHO")){
        led("vermelho");

        //impulso para frente
        rstTimer();
        while(cor(3) != "BRANCO"){
            motor(150,150);
            delay(50);

            if(timer() >= 1000){
                goto saida;
            }
        }

        

        rstTimer();

        //virar até encontrar a linha
        while(cor(3) != "PRETO"){
            motor(1000,-1000);

            if(luz(1) <= linha || luz(2) <= linha){
                rot(1000, 5);
                
                goto saida;
            }

            //Timer para correção
            if(timer() >= 2000){
                
                rstTimer();
                motor(300,300);
                delay(100);

                while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                    motor(-1000,1000);

                    //Timer para correção
                    if(timer() >= 2500){
                        rot(500, -20);

                        while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                            motor(-100,-100);
                        }

                        motor(-200,-200);
                        delay(200);

                        rot(500,-5);
                    }
                }
                break;
            }
        }

        ledOff();
    }        

    //Virar para Direita
    if((luz(1) < linha || luz(2) < linha) && (luz(5) > linha) && (cor(1) != "VERMELHO" || cor(5) != "VERMELHO")){
        led("azul");
    
        //impulso para frente
        rstTimer();
        while(cor(3) != "BRANCO"){
            motor(150,150);
            delay(50);

            if(timer() >= 1000){
                goto saida;
            }
        }

        rstTimer();

        //virar até encontrar a linha
        while(cor(3) != "PRETO"){
            motor(-1000,1000);

            if(luz(4) <= linha || luz(5) <= linha){
                rot(1000, -5);
                
                goto saida;
            }

            //Timer para correção
            if(timer() >= 2000){
                
                rstTimer();
                motor(300,300);
                delay(100);

                while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                    motor(1000,-1000);

                    //Timer para correção
                    if(timer() >= 2500){
                        rot(500, 20);

                        while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                            motor(-100,-100);
                        }

                        motor(-200,-200);
                        delay(200);

                        rot(500,5);
                    }
                }
                break;
            }
        }

        ledOff();
    }

}

void Verde1(){
    int tempoDeIda = 450;
    float verde = 70;

    saida:

    //Verde no meio
    if(cor(3) == "VERDE"){
        led("verde");
        print(0, "VERDE meio");
        
        while(cor(3) == "VERDE"){
            motor(-300,-300);
        }
        motor(-300,-300);
        delay(200);

        rot(1000, 5);

        printClear();
        ledOff();
    }

    //Verde na Esquerda
    if ((corR(4) < corG(4) && corB(4) < corG(4) && corG(4) > verde) || (corR(5) < corG(5) && corB(5) < corG(5) && corG(5) > verde)){
        led("verde");
        print(0, "<= VERDE");

        rot(1000,-2);

        //verificar 2 verdes
        if((cor(1) == "VERDE" || cor(2) == "VERDE") || (cor(1) == "AMARELO" || cor(2) == "AMARELO")){
            print(1, "Dois Verdes");
            rot(1000,150);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            

            doisVerdes = true;
        }

        //não tem 2 verdes
        if(!doisVerdes){
           
            rot(1000,2);

            //ir reto até deixar de ver a linha
            rstTimer();
            while(cor(5) != "PRETO" && cor(4) != "PRETO" && cor(2) != "PRETO" && cor(1) != "PRETO"){        
                motor(50,50);  

                if(timer() >= 1000){
                    goto saida;
                }

            }

            motor(1000, 1000);
            delay(tempoDeIda);  //470

            rot(500, -15);

            //girar ate achar a linha 
            while (cor(3) != "PRETO")
            {
                motor(1000, -1000);
            }

            motor(-1000, -1000);
            delay(100);

            printClear();
            ledOff();
        }     

        printClear();
        ledOff();
        doisVerdes = false;
    }

    //Verde na Direita
    if ((corR(1) < corG(1) && corB(1) < corG(1) && corG(1) > verde) || (corR(2) < corG(2) && corB(2) < corG(2) && corG(2) > verde)){
        led("verde");
        print(0, "VERDE =>");
        
        rot(1000,2);

        //verificar 2 verdes
        if((cor(4) == "VERDE" || cor(5) == "VERDE") || (cor(4) == "AMARELO" || cor(5) == "AMARELO")){
            print(1, "Dois Verdes");
            rot(1000,150);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            doisVerdes = true;
        }
        
        if(!doisVerdes){
            
            rot(1000,-2);

            rstTimer();
            while(cor(5) != "PRETO" && cor(4) != "PRETO" && cor(2) != "PRETO" && cor(1) != "PRETO"){
                motor(50,50);

                if(timer() >= 1000){
                    goto saida;
                }  
            }

            motor(1000, 1000);
            delay(tempoDeIda);  //470

            rot(500, 15);
            
            //girar ate achar a linha 
            while (cor(3) != "PRETO")
            {
                motor(-1000, 1000);
            }

            motor(-1000, -1000);
            delay(100);

            printClear();
            ledOff();
        }

        printClear();
        ledOff();
        doisVerdes = false;
    }

}

void Verde2(){
    int tempoDeIda = 450;
    float verde = 70;

    saida:

    //Verde no meio
    if(cor(3) == "VERDE"){
        led("verde");
        print(0, "VERDE meio");
        
        while(cor(3) == "VERDE"){
            motor(-300,-300);
        }
        motor(-300,-300);
        delay(200);

        rot(1000, 5);

        printClear();
        ledOff();
    }

    //Verde na Esquerda
    if (cor(5) == "VERDE" || cor(4) == "VERDE" || cor(5) == "AMARELO" || cor(4) == "AMARELO"){
        led("verde");
        print(0, "<= VERDE");

        rot(1000,-2);

        //verificar 2 verdes
        if((cor(1) == "VERDE" || cor(2) == "VERDE") || (cor(1) == "AMARELO" || cor(2) == "AMARELO")){
            print(1, "Dois Verdes");
            rot(1000,150);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            doisVerdes = true;
        }

        //não tem 2 verdes
        if(!doisVerdes){
           
            rot(1000,2);

            //ir reto até deixar de ver a linha
            rstTimer();
            while(cor(5) != "PRETO" && cor(4) != "PRETO" && cor(2) != "PRETO" && cor(1) != "PRETO"){        
                motor(50,50);  

                if(timer() >= 1000){
                    goto saida;
                }

            }

            motor(1000, 1000);
            delay(tempoDeIda);  //470

            rot(500, -15);

            //girar ate achar a linha 
            while (cor(3) != "PRETO")
            {
                motor(1000, -1000);
            }

            motor(-1000, -1000);
            delay(100);

            printClear();
            ledOff();
        }     

        printClear();
        ledOff();
        doisVerdes = false;
    }

    //Verde na Direita
    if (cor(2) == "VERDE" || cor(1) == "VERDE" || cor(2) == "AMARELO" || cor(1) == "AMARELO"){
        led("verde");
        print(0, "VERDE =>");
        
        rot(1000,2);

        //verificar 2 verdes
        if((cor(4) == "VERDE" || cor(5) == "VERDE") || (cor(4) == "AMARELO" || cor(5) == "AMARELO")){
            print(1, "Dois Verdes");
            rot(1000,150);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            doisVerdes = true;
        }
        
        if(!doisVerdes){
            
            rot(1000,-2);

            rstTimer();
            while(cor(5) != "PRETO" && cor(4) != "PRETO" && cor(2) != "PRETO" && cor(1) != "PRETO"){
                motor(50,50);

                if(timer() >= 1000){
                    goto saida;
                }  
            }

            motor(1000, 1000);
            delay(tempoDeIda);  //470

            rot(500, 15);
            
            //girar ate achar a linha 
            while (cor(3) != "PRETO")
            {
                motor(-1000, 1000);
            }

            motor(-1000, -1000);
            delay(100);

            printClear();
            ledOff();
        }

        printClear();
        ledOff();
        doisVerdes = false;
    }

}

void Obstaculo(){
    
    if (sonar(2) < 11){
        
        led("amarelo");
        print(0, "OBSTÁCULO");

        motor(-200,-200);
        delay(200);

        motor(0,0);

        //correção de giro 45°
        while(true){
            // NORTE 
            if ((compass() >= 0 && compass() < 45) || (compass() > 315 && compass() <= 360)){            
                print(1, "NORTE ATIVADO");
                rot(1000, 15);
                
                while (compass() < 40){
                    motor(-1000, 1000);
                } 
                break;
            }
            // OESTE
            else if(compass() > 225 && compass() < 315){
                print(1, "OESTE ATIVADO");
                rot(1000, 15);

                while (compass() < 310){
                    motor(-1000, 1000);
                } 
                break;
            }
            // SUL
            else if(compass() > 135 && compass() < 225){
                print(1, "SUL ATIVADO");
                rot(1000, 15);
                
                while (compass() < 220){
                    motor(-1000, 1000);
                } 
                break;
            }  
            // LESTE
            else if(compass() > 45 && compass() < 135){   
                print(1, "LESTE ATIVADO");
                rot(1000, 15);

                while (compass() < 130){
                    motor(-1000, 1000);
                } 
                break;
            } 
        }

        //condição 1
        rstTimer();
        while (true){

            motor(200, 200); //150 150

            if (luz(5) < linha || luz(1) < linha){
                
                print(1,"condi 1");
                
                motor(1000, 1000);
                delay(550);
                
                rot(500, 40);

                while(!bc.touch(0)){
                    motor(-200,-200);
                }
                
                break;
            }

            if (timer() >= 1000){
                
                motor(-200, -200);
                delay(1000);

                rot(500, -78);
                
                rstTimer();

                //condição 2
                while (true){
                    motor(200, 200);//150 150

                    if (luz(5) < linha || luz(1) < linha){
                        print(1,"condi 2");
                        
                        motor(1000, 1000);
                        delay(550);
                        
                        rot(500, -50);

                        while(!bc.touch(0)){
                            motor(-200,-200);
                        }
                        
                        break;
                    }                   
                    
                    if (timer() >= 1200){   
                        print(1,"condi 3");
                        
                        rot(1000, 30);
                        
                        motor(500,500);
                        delay(1000); //ajustavel

                        rot(1000, 70);

                        while(luz(3) >= linha){
                            motor(100,100);
                        }

                        motor(1000, 1000);
                        delay(450);

                        rot(1000, -10);

                        //correção de giro
                        while(true){
                            
                            //LESTE ==> NORTE
                            if(compass() > 0 && compass() < 90 ){

                                while(compass() >= 1){
                                    motor(1000,-1000);
                                }

                                break;
                            }

                            //SUL ==> LESTE
                            if(compass() > 90 && compass() < 180 ){

                                while(compass() >= 90){
                                    motor(1000,-1000);
                                }

                                break;
                            }

                            //OESTE ==> SUL
                            if(compass() > 180 && compass() < 270 ){

                                while(compass() >= 180){
                                    motor(1000,-1000);
                                }

                                break;
                            }

                            //NORTE ==> OESTE
                            if(compass() > 270 && compass() < 360 ){

                                while(compass() >= 270){
                                    motor(1000,-1000);
                                }

                                break;
                            }
                        }
                                        
                        while(!bc.touch(0)){
                            motor(-200,-200);
                        }

                        motor(0,0);
                        break;
                    }
                }

                break;
            }
        }

        printClear();
        ledOff();

    }
}

void VerifSuperGap(float tempoParaSuperGap){

    if(cor(1) != "PRETO" && cor(2) != "PRETO" && cor(3) != "PRETO" && cor(4) != "PRETO" && cor(5) != "PRETO"){
        
        rstTimer();
        print(1, "SuperGap = ...");

        while(true){
            
            motor(velocidade,velocidade);

            if(cor(1) != "BRANCO" || cor(2) != "BRANCO" || cor(3) != "BRANCO" || cor(4) != "BRANCO" || cor(5) != "BRANCO"){
                motor(0,0);
                break;
            }

            //ativar super gap
            if(timer()>= tempoParaSuperGap){
                
                led("branco");
                print(1, "SuperGap = Ativado");

                SuperGap();
                
                break;
            }

            if(sonar(2) < 15){
                motor(0,0);
                break;
            }
        }

        printClear();
        ledOff();
    }
}

void SuperGap(){

    rstTimer();

    //verificando linha na Esquerda
    while(cor(1) != "PRETO" && cor(2) != "PRETO" && cor(3) != "PRETO" && cor(4) != "PRETO" && cor(5) != "PRETO"){
        motor(-1000,1000);

        //nao tem linha na esquerda
        if(timer() >= 3000){//tempo ajustavel
            
            rstTimer();

            //verificando linha na Direita
            while(cor(1) != "PRETO" && cor(2) != "PRETO" && cor(3) != "PRETO" && cor(4) != "PRETO" && cor(5) != "PRETO"){
                motor(1000,-1000);

                //nao tem linha na Direita
                if(timer() >= 6000){//tempo ajustavel
                    
                    rot(1000,75);//angulo ajustavel

                    //voltar para a linha de costas
                    while(cor(1) != "PRETO" && cor(2) != "PRETO" && cor(3) != "PRETO" && cor(4) != "PRETO" && cor(5) != "PRETO"){
                        motor(-100,-100);   
                    }

                    motor(-200,-200);
                    delay(200);

                    rot(1000, 5);

                    break;
                }
            }

            break;
        }
    }
}

void VerifRampa(){

    if(sonar(1) < 40 && inclina() >= 330 && inclina() <= 350){
        rstTimer();

        while(sonar(1) < 40 && inclina() >= 330 && inclina() <= 350){
            saida:
            motor(300,300);

            //Verificação de rampa por tempo
            if(timer() >= 2500){
                //SUBIR: 160 - 132 DESCER: 78 - 40   ATUATOR: 160 - 78   SCOOP: 132 - 40
                Descer(147,132);
                
                print(1, "Rampa Detectada");
                led("amarelo");
                
                //correção de giro na Rampa
                while(true){
                    // NORTE 
                    if ((compass() >= 0 && compass() < 45) || (compass() > 315 && compass() <= 360)){
                        print(1, "NORTE ATIVADO");
                        
                        if(compass() >= 0 && compass() < 45){  
                            do{
                                motor(-1000, 1000);
                            }while (compass() <= 1);
                        }
                        else if(compass() > 315 && compass() <= 360){  
                            do{
                                motor(1000, -1000);
                            }while (compass() >= 360);
                        }

                        break;
                    }

                    // OESTE
                    else if (compass() > 225 && compass() < 315){
                        print(1, "OESTE ATIVADO");
                        do{
                            motor(-1000, 1000);
                        } while (compass() <= 270);
                        break;
                    }

                    // SUL
                    else if (compass() > 135 && compass() < 225){
                        print(1, "SUL ATIVADO");
                        do{
                            motor(-1000, 1000);
                        } while (compass() <= 180);
                        break;
                    }

                    // LESTE
                    else if (compass() > 45 && compass() < 135){
                        print(1, "LESTE ATIVADO");
                        do{
                            motor(-1000, 1000);
                        } while (compass() <= 90);
                        break;
                    }
                }           

                printClear();

                led("amarelo");
                motor(0,0);

                InitResgate();
            }

            //Virar para Esquerda
            if((luz(5) < linha || luz(4) < linha) && (luz(1) > linha) && (cor(1) != "VERMELHO" || cor(5) != "VERMELHO")){
                led("vermelho");

                //impulso para frente
                while(cor(3) != "BRANCO"){
                    motor(150,150);
                }


                //virar até encontrar a linha
                while(cor(3) != "PRETO"){
                    motor(1000,-1000);

                    if(luz(1) <= linha || luz(2) <= linha){
                        rot(1000, 5);
                        
                        goto saida;
                    }
                }

                ledOff();
            }        

            //Virar para Direita
            if((luz(1) < linha || luz(2) < linha) && (luz(5) > linha) && (cor(1) != "VERMELHO" || cor(5) != "VERMELHO")){
                led("azul");
            
                //impulso para frente

                while(cor(3) != "BRANCO"){
                    motor(150,150);

                }

                //virar até encontrar a linha
                while(cor(3) != "PRETO"){ 
                    motor(-1000,1000);

                    if(luz(4) <= linha || luz(5) <= linha){
                        rot(1000, -5);
                        
                        goto saida;
                    }      
                }

                ledOff();
            }

        }
    }
    rstTimer();
}



//* Funções Principais para resgate
void InitResgate(){

    //correção de giro
    while(true){
        
        //Sul
        if(compass() > 135 && compass() < 225){
            while(compass() > 180 ){
                motor(1000,-1000);
            }
            while(compass() < 180 ){
                motor(-1000,1000);
            }
            break;
        }
        
        //Leste
        if(compass() > 45 && compass() < 135){
            while(compass() > 90 ){
                motor(1000,-1000);
            }
            while(compass() < 90 ){
                motor(-1000,1000);
            }
            break;
        }
        
        //Norte
        if(compass() < 45 || compass() > 315){
            while(compass() > 0.2 && compass() < 45){
                motor(1000,-1000);
            }
            while(compass() > 270 && compass() < 359.8){
                motor(-1000,1000);
            }
            break;
        }
        
        //Oeste
        if(compass() > 225 && compass() < 315){
            while(compass() > 270 ){
                motor(1000,-1000);
            }
            while(compass() < 270 ){
                motor(-1000,1000);
            }
            break;
        }
    }

    //ir para frente até não ver rampa
    while(sonar(1) < 40){
        motor(300,300);
    }
    motor(0,0);
    Subir(159, 130);
    
    //correção de giro
    while(true){
        
        //Sul
        if(compass() > 135 && compass() < 225){
            while(compass() > 180 ){
                motor(1000,-1000);
            }
            while(compass() < 180 ){
                motor(-1000,1000);
            }
            break;
        }
        
        //Leste
        if(compass() > 45 && compass() < 135){
            while(compass() > 90 ){
                motor(1000,-1000);
            }
            while(compass() < 90 ){
                motor(-1000,1000);
            }
            break;
        }
        
        //Norte
        if(compass() < 45 || compass() > 315){
            while(compass() > 0.2 && compass() < 45){
                motor(1000,-1000);
            }
            while(compass() > 270 && compass() <= 360){
                motor(-1000,1000);
            }
            break;
        }
        
        //Oeste
        if(compass() > 225 && compass() < 315){
            while(compass() > 270 ){
                motor(1000,-1000);
            }
            while(compass() < 270 ){
                motor(-1000,1000);
            }
            break;
        }
    }

    //ler valores
    {
        pos1 = sonar(1);

        motor(200,200);
        delay(50);

        pos2 = sonar(1);

        motor(200,200);
        delay(50);

        pos3 = sonar(1);

        print(3, pos1.ToString() + "   " + pos2.ToString() + "   " + pos3.ToString());
    }
    
    //detectar resgate posição 3 após sair da rampa
    if(!((pos2 - pos1) <= 0.1 && (pos2 - pos1) >= -0.1)){
    
        if(((pos2 - pos1) - (pos3 - pos2) > -1 && (pos2 - pos1) - (pos3 - pos2) < 1) && sonar(1) > 125 && sonar(1) < 230){
            print(0, "Resgate 3 ATIVADO");
            resgatePos3 = true;
        }
    }
    
    motor(300,300);
    delay(300);
    rot(1000,5);

    anguloInicial = compass();
    print(2, "Angulo Inicial:" + "  " + compass().ToString());
    motor(0,0);
    
    //!VERIFICAR SAIDAS
        //verifica saidas pos1 pos2 pos3
        led("ciano");
        rstTimer();
        while(compass() - anguloInicial <= 60){
            motor(-1000,1000);

            if(sonar(0) >= 9000){
                saidaPos2 = true;
                break;
            }

            if(timer() >= 10000){
                break;
            }
        }

        rstTimer();
        if(saidaPos2 == false){
            if(anguloInicial >= 270){
                while(compass() - anguloInicial <= 90){
                    motor(-1000,1000);

                    if(sonar(0) >= 9000){
            
                        saidaPos3 = true;
                        break;
                    }
    
                    if(timer() >= 5000){
                        break;
                    }
                    
                    if(anguloInicial >= 270 && compass() < 270){
                        break;
                    }
                    if(compass() >= 359){
                        break;
                    }
                }

            }else{
                while(compass() - anguloInicial <= 90){
                    motor(-1000,1000);

                    if(sonar(0) >= 9000){
            
                        saidaPos3 = true;
                        break;
                    }
    
                    if(timer() >= 10000){
                        break;
                    }
                }
            }


        }
        if(saidaPos2 == false && saidaPos3 == false){saidaPos1 = true;}

        print(1, saidaPos1.ToString() + " / " + saidaPos2.ToString() + " / " + saidaPos3.ToString());

        //voltar para posição inicial
        anguloParaVoltar = compass() - anguloInicial;
        rot(1000, -anguloParaVoltar);
        //correção de giro RETO
        while(true){
                    
            led("vermelho");
            
            //Sul
            if(compass() > 135 && compass() < 225){
                while(compass() > 180 ){
                    motor(1000,-1000);
                }
                while(compass() < 180 ){
                    motor(-1000,1000);
                }
                break;
            }
            //Leste
            if(compass() > 45 && compass() < 135){
                while(compass() > 90 ){
                    motor(1000,-1000);
                }
                while(compass() < 90 ){
                    motor(-1000,1000);
                }
                break;
            }
            //Norte
            if(compass() < 45 || compass() > 315){
                while(compass() > 1 && compass() < 45){
                    motor(1000,-1000);
                }
                while(compass() < 360 && compass() > 315){
                    motor(-1000,1000);
                }
                break;
            }
            //Oeste
            if(compass() > 225 && compass() < 315){
                while(compass() > 270 ){
                    motor(1000,-1000);
                }
                while(compass() < 270 ){
                    motor(-1000,1000);
                }
                break;
            }
        }
        motor(0,0);
        led("amarelo");
         
    
    //!VERIFICAR RESGATES

        //detectar resgate posição 3 enquanto está andando
        if(!resgatePos3 && sonar(0) > 186){
            
            //detectar resgate posição 3 
            while(sonar(0) > 186){

                motor(200,200); //?velocidade ajustável (detectar resgate)

                pos1 = pos2;
                pos2 = pos3;
                pos3 = sonar(1);

                //verif pos1 pos2 pos3 
                if(!((pos2 - pos1) <= 0.1 && (pos2 - pos1) >= -0.1)){
                    
                    //verif resgate posição 3 usando pos1 pos2 pos3
                    if(((pos2 - pos1) - (pos3 - pos2) > -1 && (pos2 - pos1) - (pos3 - pos2) < 1) && sonar(1) > 125 && sonar(1) < 230){
                        resgatePos3 = true;
                        
                        break;
                    }

                    {/*//bolinha no lado??
                    //!WIP
                    else if(!bc.HasVictim() && sonar(1) > 17){
                        led("verde");

                        //impulso
                        motor(200,200);
                        delay(200);

                        //ficar de frente para bolinha
                        rot(1000, 90);

                        //ir um pouco para trás
                        motor(-200,-200);
                        delay(1000);
                        
                        //ir até a bolinha
                        rstTimer();
                        while(sonar(2) > 10 && sonar(0) > 5){
                            motor(1000,1000);

                            if(timer() >= 3000){
                                break;
                            }
                        }

                        //pegar bolinha
                        if(sonar(2) < 15 && sonar(0) > 5){
                            PegarBolinha();
                            
                            led("vermelho");
                        }

                        //ta com bolinha?
                        if(bc.HasVictim()){
                            
                            //voltar para parede
                            while(!bc.touch(0)){
                                motor(-150,-150);

                                if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                                    motor(200,200);
                                    delay(1400);
                                    break;
                                }
                            }
                            
                            rot(1000,-90);
                        }
                        else{
                            //voltar para parede
                            while(!bc.touch(0)){
                                motor(-1000,-1000);

                                if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                                    motor(200,200);
                                    delay(1400);
                                    break;
                                }
                            }
                            
                            rot(1000,-90);
                        }
                    
                        //!termino do if
                    }*/}
            
                }
            }
        }
        
        //detectar resgate posição 1
        if(!resgatePos3 && !resgatePos1){
            while(true){ 
                //ir pra frente
                motor(200,200); //?velocidade ajustável (detectar resgate)

                //verif resgate pos 1
                if((sonar(0) - sonar(2) >= 20) && sonar(0) < 120 && cor(6) == "PRETO"){
                    resgatePos1 = true;
                    break;
                }

                //verif parede / resgate pos 2 WIP
                if(sonar(0) < 20 && sonar(2) < 5){     
                    resgatePos2 = true;
                    rot(1000,90);
                    //correção de giro RETO
                    while(true){
                                
                        led("vermelho");
                        
                        //Sul
                        if(compass() > 135 && compass() < 225){
                            while(compass() > 180 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 180 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Leste
                        if(compass() > 45 && compass() < 135){
                            while(compass() > 90 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 90 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Norte
                        if(compass() < 45 || compass() > 315){
                            while(compass() > 1 && compass() < 45){
                                motor(1000,-1000);
                            }
                            while(compass() < 360 && compass() > 315){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Oeste
                        if(compass() > 225 && compass() < 315){
                            while(compass() > 270 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 270 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                    }
                    
                    break;
                }

                //procurar/pegar bolinha
                if(!bc.HasVictim() && sonar(1) < 120 && sonar(1) > 17){
                    
                    InitBolinhaNoLado(false);

                    //pegar bolinha
                    if(sonar(0) - sonar(2) >= 17 && sonar(2) < 40){
                        PegarBolinha();               
                        led("vermelho");
                    }
                    
                    //ta com bolinha?
                    if(bc.HasVictim()){
                        
                        //voltar para parede
                        for(int a = 100; a <= 200; a++){ //?TESTE
                            motor(-a,-a);
                            delay(1);
                        }
                        while(!bc.touch(0)){
                            motor(-200,-200); //?velocidade ajustável (voltar para parede)

                            if(sonar(0) >= 244){
                                break;
                            }
                        }
                        
                        rot(1000,-90);
                        //correção de giro RETO
                        while(true){
                                    
                            led("vermelho");
                            
                            //Sul
                            if(compass() > 135 && compass() < 225){
                                while(compass() > 180 ){
                                    motor(1000,-1000);
                                }
                                while(compass() < 180 ){
                                    motor(-1000,1000);
                                }
                                break;
                            }
                            //Leste
                            if(compass() > 45 && compass() < 135){
                                while(compass() > 90 ){
                                    motor(1000,-1000);
                                }
                                while(compass() < 90 ){
                                    motor(-1000,1000);
                                }
                                break;
                            }
                            //Norte
                            if(compass() < 45 || compass() > 315){
                                while(compass() > 1 && compass() < 45){
                                    motor(1000,-1000);
                                }
                                while(compass() < 360 && compass() > 315){
                                    motor(-1000,1000);
                                }
                                break;
                            }
                            //Oeste
                            if(compass() > 225 && compass() < 315){
                                while(compass() > 270 ){
                                    motor(1000,-1000);
                                }
                                while(compass() < 270 ){
                                    motor(-1000,1000);
                                }
                                break;
                            }
                        }
                                
                    }
                    else{
                        //voltar para parede
                        while(!bc.touch(0)){
                            motor(-1000,-1000);

                            if(sonar(0) >= 244){
                                break;
                            }
                        }
                        
                        rot(1000,-90);
                        //correção de giro RETO
                        while(true){
                                    
                            led("vermelho");
                            
                            //Sul
                            if(compass() > 135 && compass() < 225){
                                while(compass() > 180 ){
                                    motor(1000,-1000);
                                }
                                while(compass() < 180 ){
                                    motor(-1000,1000);
                                }
                                break;
                            }
                            //Leste
                            if(compass() > 45 && compass() < 135){
                                while(compass() > 90 ){
                                    motor(1000,-1000);
                                }
                                while(compass() < 90 ){
                                    motor(-1000,1000);
                                }
                                break;
                            }
                            //Norte
                            if(compass() < 45 || compass() > 315){
                                while(compass() > 1 && compass() < 45){
                                    motor(1000,-1000);
                                }
                                while(compass() < 360 && compass() > 315){
                                    motor(-1000,1000);
                                }
                                break;
                            }
                            //Oeste
                            if(compass() > 225 && compass() < 315){
                                while(compass() > 270 ){
                                    motor(1000,-1000);
                                }
                                while(compass() < 270 ){
                                    motor(-1000,1000);
                                }
                                break;
                            }
                        }
                               
                    }
                }
            
                //procurar/pegar bolinha de frente
                if(!bc.HasVictim() && sonar(2) < 8 && sonar(0) > 20){
                    led("verde");

                    PegarBolinhaDeFrente();

                    led("vermelho");


                }
            }
        }


    //!ATIVAR RESGATES
        //*RESGATE POSIÇÃO 1
        if(resgatePos1 && !resgatePos3){
            print(0, "Resgate 1 ATIVADO");
            ResgatePosição1();
        }

        //*RESGATE POSIÇÃO 2
        if(!resgatePos1 && !resgatePos3){
            print(0, "Resgate 2 ATIVADO");
            ResgatePosição2();
        }

        //*RESGATE POSIÇÃO 3
        if(resgatePos3 && !resgatePos1){
            print(0, "Resgate 3 ATIVADO");
            ResgatePosição3();
        }
        

    //!Final do Resgate
        led("vermelho");
        motor(0,0);
        delay(100000);
}

void ResgatePosição1(){
    motor(0,0);
    led("amarelo");

    //*ja tem bolinha na garra
    if(bc.HasVictim()){
        
        EntregarBolinha();

        //correção de giro RETO
        while(true){
                    
            led("vermelho");
            
            //Sul
            if(compass() > 135 && compass() < 225){
                while(compass() > 180 ){
                    motor(1000,-1000);
                }
                while(compass() < 180 ){
                    motor(-1000,1000);
                }
                break;
            }
            //Leste
            if(compass() > 45 && compass() < 135){
                while(compass() > 90 ){
                    motor(1000,-1000);
                }
                while(compass() < 90 ){
                    motor(-1000,1000);
                }
                break;
            }
            //Norte
            if(compass() < 45 || compass() > 315){
                while(compass() > 1 && compass() < 45){
                    motor(1000,-1000);
                }
                while(compass() < 360 && compass() > 315){
                    motor(-1000,1000);
                }
                break;
            }
            //Oeste
            if(compass() > 225 && compass() < 315){
                while(compass() > 270 ){
                    motor(1000,-1000);
                }
                while(compass() < 270 ){
                    motor(-1000,1000);
                }
                break;
            }
        }
    
    }
    
    //*Procurar bolinha...
    ResgateDeRé();
}

void ResgatePosição2(){
    motor(0,0);
    led("amarelo");

    //*ja tem bolinha na garra
    if(bc.HasVictim()){
        
        //ir ate o resgate pos 2
        while(luz(6) > 10 && sonar(0) > 25){
            motor(300,300); //?velocidade ajustável (ir ate o resgate pos 2)
        }

        EntregarBolinha();

        //correção de giro RETO
        while(true){
                    
            led("vermelho");
            
            //Sul
            if(compass() > 135 && compass() < 225){
                while(compass() > 180 ){
                    motor(1000,-1000);
                }
                while(compass() < 180 ){
                    motor(-1000,1000);
                }
                break;
            }
            //Leste
            if(compass() > 45 && compass() < 135){
                while(compass() > 90 ){
                    motor(1000,-1000);
                }
                while(compass() < 90 ){
                    motor(-1000,1000);
                }
                break;
            }
            //Norte
            if(compass() < 45 || compass() > 315){
                while(compass() > 1 && compass() < 45){
                    motor(1000,-1000);
                }
                while(compass() < 360 && compass() > 315){
                    motor(-1000,1000);
                }
                break;
            }
            //Oeste
            if(compass() > 225 && compass() < 315){
                while(compass() > 270 ){
                    motor(1000,-1000);
                }
                while(compass() < 270 ){
                    motor(-1000,1000);
                }
                break;
            }
        }

        ResgateDeRé();
    }
    
    //*Procurar bolinha...
    while(resgatePos2){
        voltarProcurarBolinha:
        
        //ir pra frente
        motor(300,300); //?velocidade ajustável (ir pra frente)

        //verif. bolinha no lado
        if(sonar(1) < 240 && sonar(1) > 17){   
            
            InitBolinhaNoLado(false);
            
            //pegar bolinha
            if((sonar(0) - sonar(2)) >= 17 && sonar(2) < 40){
                PegarBolinha();
                led("vermelho");
            } 

            //ta com a bolinha
            if(bc.HasVictim()){

                for(int a = 100; a <= 200; a++){ //?TESTE
                    
                    motor(-a,-a);
                    delay(1);

                }
                
                //voltar para parede
                while(!bc.touch(0)){
                    motor(-200,-200); //?velocidade ajustável (voltar para parede)

                    if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                        motor(200,200);
                        delay(1400);
                        break;
                    }
                }
            
                //correção de Giro 90°
                rot(1000,-70);
                while(true){
                                            
                    //Sul
                    if(compass() > 180 && compass() < 270){
                        while(compass() > 180 ){
                            motor(1000,-1000);
                        }
                        break;
                    }
                    //Leste
                    if(compass() > 90 && compass() < 180){
                        while(compass() > 90 ){
                            motor(1000,-1000);
                        }
                        break;
                    }
                    //Norte
                    if(compass() > 1 && compass() < 90){
                        while(compass() > 1 ){
                            motor(1000,-1000);
                        }
                        break;
                    }
                    //Oeste
                    if(compass() > 270 && compass() < 360){
                        while(compass() > 270 ){
                            motor(1000,-1000);
                        }
                        break;
                    }
                }

                //ir para resgate
                while(luz(6) > 10 && sonar(0) > 25){
                    motor(300,300); //?velocidade ajustável (ir para resgate)
                }

                EntregarBolinha(); 

                //correção de giro RETO
                while(true){
                    
                    led("vermelho");
                    
                    //Sul
                    if(compass() > 135 && compass() < 225){
                        while(compass() > 180 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 180 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Leste
                    if(compass() > 45 && compass() < 135){
                        while(compass() > 90 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 90 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Norte
                    if(compass() < 45 || compass() > 315){
                        while(compass() > 1 && compass() < 45){
                            motor(1000,-1000);
                        }
                        while(compass() < 360 && compass() > 315){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Oeste
                    if(compass() > 225 && compass() < 315){
                        while(compass() > 270 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 270 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                }

                ResgateDeRé();
            }
            //não ta com a bolinha
            else{
                //voltar para parede
                while(!bc.touch(0)){
                    motor(-1000,-1000);

                    if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                        motor(200,200);
                        delay(1400);
                        break;
                    }
                }
                
                rot(1000,-90);
                led("amarelo");
            }
        
        }
   
        //verif. bolinha na frente
        if(sonar(2) < 8 && sonar(0) > 20){
            
            led("verde");

            PegarBolinhaDeFrente();

            led("vermelho");
            
            //ta com a bolinha
            if(bc.HasVictim()){
                
                //ir ate o resgate pos 2
                while(luz(6) > 10 && sonar(0) > 25){
                    motor(500,500); //?velocidade ajustável (ir ate o resgate pos 2)
                }

                led("verde");
                motor(0,0);

                //gira caso não tenha espaço
                if(sonar(0) < 70){
                    giro = true;
                    rot(1000, 45);
                    motor(0,0);
                }
                
                EntregarBolinha();

                //correção de giro RETO
                while(true){
                    
                    led("vermelho");
                    
                    //Sul
                    if(compass() > 135 && compass() < 225){
                        while(compass() > 180 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 180 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Leste
                    if(compass() > 45 && compass() < 135){
                        while(compass() > 90 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 90 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Norte
                    if(compass() < 45 || compass() > 315){
                        while(compass() > 1 && compass() < 45){
                            motor(1000,-1000);
                        }
                        while(compass() < 360 && compass() > 315){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Oeste
                    if(compass() > 225 && compass() < 315){
                        while(compass() > 270 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 270 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                }

                led("amarelo");

                ResgateDeRé();
            }
            //não ta com a bolinha
            else{
                motor(-100,-100);
                delay(500);

                goto voltarProcurarBolinha;
            }
        }
   
        //procurar de ré AGORA
        if(luz(6) < 10 && sonar(0) < 90 && sonar(0) > 70){
            
            //correção de giro RETO
            while(true){
                
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            ResgateDeRé();
        }
    }
}

void ResgatePosição3(){
    
    //*ja tem bolinha na garra
    if(bc.HasVictim()){
        rot(1000, 90);

        //entregar direto
        if(distancia == 120){
            print(3, "entregar direto");
            
            //ir ate o resgate pos3
            while(luz(6) > 10 && sonar(0) > 25){
                motor(500,500); //?velocidade ajustável (ir ate o resgate pos 3)
            }

            led("verde");
            motor(0,0);

            //gira caso não tenha espaço
            if(sonar(0) < 70){
                giro = true;
                rot(1000, 45);
                motor(0,0);
            }
            
            EntregarBolinha();
            
            //giro para ficar de costas para parede
            if(giro){
                rot(1000, 90);
                motor(0,0);
            }
            else{
                rot(1000, 135);
                motor(0,0);
            }

            //tocar na parede atrás
            rstTimer();
            while(timer() < 2000){
                motor(-1000,-1000); //?velocidade ajustável (tocar na parede atrás)
            }

            //correção de Giro 45°
            while(true){
                
                led("vermelho");
                
                //Sul
                if(compass() > 180 && compass() < 270){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 90 && compass() < 180){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    break;
                }
                //Norte
                if(compass() > 1 && compass() < 90){
                    while(compass() > 1 ){
                        motor(1000,-1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 270 && compass() < 360){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    break;
                }
            }

            led("amarelo");

            ResgateDeRé();
        }
        
        //entregar virando na parede
        else if(distancia == 225){
            print(3, "entregar virando na parede");

            //ir ate a parede
            while(sonar(0) > 16){
                motor(500,500); //?velocidade ajustável (ir ate a parede)
            }

            //virar
            rot(1000, 90);

            //ir reto ate achar zona de resgate
            while(luz(6) > 10 && sonar(0) > 70){
                motor(500,500); //?velocidade ajustável (ir reto ate achar zona de resgate)
            }

            led("verde");
            motor(0,0);
            
            EntregarBolinha();

            led("amarelo");

            ResgateDeRé();
        }
    }
    
    //*Procurar bolinha...
    voltarProcurarBolinha:
    led("amarelo");
    while(resgatePos3){
           
        //ir pra frente
        motor(300,300);  //?velocidade ajustável (ir pra frente)

        //area de resgate longe
        if(sonar(0) < 165){distancia = 220;}
        
        //verif. bolinha no lado //distancia = 120
        if(sonar(1) < distancia && sonar(1) > 17){   
            
            InitBolinhaNoLado(false);

            //pegar bolinha
            if((sonar(0) - sonar(2)) >= 17 && sonar(2) < 40){
                PegarBolinha();
                led("vermelho");
            }
            else{
                rstTimer();
    
                while((sonar(0) - sonar(2)) <= 20 || sonar(2) > 30){
                    motor(-1000,1000);

                    if(timer() >= 1500){
                        rstTimer();
                        while((sonar(0) - sonar(2)) <= 20 || sonar(2) > 30){
                            motor(1000,-1000);

                            if(timer() >= 3000){
                                led("vermelho");
                                
                                //correção de giro
                                while(true){
                                    
                                    //Sul
                                    if(compass() > 135 && compass() < 225){
                                        while(compass() > 180 ){
                                            motor(1000,-1000);
                                        }
                                        while(compass() < 180 ){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                    
                                    //Leste
                                    if(compass() > 45 && compass() < 135){
                                        while(compass() > 90 ){
                                            motor(1000,-1000);
                                        }
                                        while(compass() < 90 ){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                    
                                    //Norte
                                    if(compass() < 45 || compass() > 315){
                                        while(compass() > 0.2 && compass() < 45){
                                            motor(1000,-1000);
                                        }
                                        while(compass() > 270 && compass() < 359.8){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                    
                                    //Oeste
                                    if(compass() > 225 && compass() < 315){
                                        while(compass() > 270 ){
                                            motor(1000,-1000);
                                        }
                                        while(compass() < 270 ){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                }

                                //voltar para parede
                                while(!bc.touch(0)){
                                    motor(-1000,-1000);

                                    if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                                        motor(200,200);
                                        delay(1400);
                                        break;
                                    }

                                    if(inclina() > 300){
                                        motor(200,200);
                                        delay(1000);
                                        break;  
                                    }
                                }
                                
                                rot(1000,-90);

                                goto voltarProcurarBolinha;
                            }
                        }

                        leftturn = true;
                        rot(1000, -10);
                        break;
                    }
                }
                if(!leftturn){rot(1000, 10);}

                led("verde");
                PegarBolinha();
                led("vermelho");

                //correção de giro
                while(true){
                    
                    //Sul
                    if(compass() > 135 && compass() < 225){
                        while(compass() > 180 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 180 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    
                    //Leste
                    if(compass() > 45 && compass() < 135){
                        while(compass() > 90 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 90 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    
                    //Norte
                    if(compass() < 45 || compass() > 315){
                        while(compass() > 0.2 && compass() < 45){
                            motor(1000,-1000);
                        }
                        while(compass() > 270 && compass() < 359.8){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    
                    //Oeste
                    if(compass() > 225 && compass() < 315){
                        while(compass() > 270 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 270 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                }
                motor(0,0);
            }

            //ta com a bolinha?
            if(bc.HasVictim()){
                
                print(3, distancia.ToString());

                //entregar direto
                if(distancia == 120){
                    print(3, "entregar direto");
                    
                    //ir ate o resgate pos 3
                    while(luz(6) > 10 && sonar(0) > 25){
                        motor(500,500); //?velocidade ajustável (ir ate o resgate pos 3)
                    }

                    led("verde");
                    motor(0,0);

                    //gira caso não tenha espaço
                    if(sonar(0) < 70){
                        giro = true;
                        rot(1000, 45);
                        motor(0,0);
                    }
                    
                    EntregarBolinha();
                    
                    //giro para ficar de costas para parede
                    if(giro){
                        rot(1000, 90);
                        motor(0,0);
                    }
                    else{
                        rot(1000, 135);
                        motor(0,0);
                    }

                    //tocar na parede atrás
                    rstTimer();
                    while(timer() < 2000){
                        motor(-1000,-1000); //?velocidade ajustável (tocar na parede atrás)
                    }

                    //correção de Giro 45°
                    while(true){
                        
                        led("vermelho");
                        
                        //Sul
                        if(compass() > 180 && compass() < 270){
                            while(compass() > 180 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                        //Leste
                        if(compass() > 90 && compass() < 180){
                            while(compass() > 90 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                        //Norte
                        if(compass() > 1 && compass() < 90){
                            while(compass() > 1 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                        //Oeste
                        if(compass() > 270 && compass() < 360){
                            while(compass() > 270 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                    }
                    ResgateDeRé();
                }
                
                //entregar virando na parede
                else if(distancia == 220){
                    print(3, "entregar virando na parede");

                    //ir ate a parede
                    while(sonar(0) > 20){
                        motor(300,300); //?velocidade ajustável (ir ate a parede)
                    }

                    //virar
                    rot(1000, 90);
                    //correção de giro RETO
                    while(true){
                                
                        led("vermelho");
                        
                        //Sul
                        if(compass() > 135 && compass() < 225){
                            while(compass() > 180 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 180 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Leste
                        if(compass() > 45 && compass() < 135){
                            while(compass() > 90 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 90 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Norte
                        if(compass() < 45 || compass() > 315){
                            while(compass() > 1 && compass() < 45){
                                motor(1000,-1000);
                            }
                            while(compass() < 360 && compass() > 315){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Oeste
                        if(compass() > 225 && compass() < 315){
                            while(compass() > 270 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 270 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                    }

                    //ir reto ate achar zona de resgate
                    while(luz(6) > 10 && sonar(0) > 70){
                        motor(500,500); //?velocidade ajustável (ir reto ate achar zona de resgate)
                    }

                    led("verde");
                    motor(0,0);
                    
                    EntregarBolinha();
                    
                    ResgateDeRé();
                }
            }
            //não ta com a bolinha 
            else{
                
                //voltar para parede
                while(!bc.touch(0)){
                    motor(-1000,-1000);

                    if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                        motor(200,200);
                        delay(1400);
                        break;
                    }
                }
                
                rot(1000,-90);
            }
        
        }

        //verif. bolinha na frente
        if(sonar(2) < 8 && sonar(0) > 20){
            
            led("verde");

            //area de resgate longe
            if(sonar(0) < 168){
                distancia = 220;
            }

            PegarBolinhaDeFrente();

            led("vermelho");

            //ta com a bolinha????
            if(bc.HasVictim()){
                
                //entregar direto
                if(distancia == 120){
                    print(3, "entregar direto");
                    
                    rot(1000, 90);
    
                    //ir ate o resgate pos 3
                    while(luz(6) > 10 && sonar(0) > 25){
                        motor(500,500); //?velocidade ajustável (ir ate o resgate pos 3)
                    }

                    led("verde");
                    motor(0,0);

                    //gira caso não tenha espaço
                    if(sonar(0) < 70){
                        giro = true;
                        rot(1000, 45);
                        motor(0,0);
                    }
                    
                    EntregarBolinha();

                    //giro para ficar de costas para parede
                    if(giro){
                        rot(1000, 90);
                        motor(0,0);
                    }
                    else{
                        rot(1000, 135);
                        motor(0,0);
                    }

                    //tocar na parede atrás
                    rstTimer();
                    while(timer() < 2000){
                        motor(-1000,-1000); //?velocidade ajustável (tocar na parede atrás)
                    }

                    //correção de Giro 45°
                    while(true){
                        
                        led("vermelho");
                        
                        //Sul
                        if(compass() > 180 && compass() < 270){
                            while(compass() > 180 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                        //Leste
                        if(compass() > 90 && compass() < 180){
                            while(compass() > 90 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                        //Norte
                        if(compass() > 1 && compass() < 90){
                            while(compass() > 1 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                        //Oeste
                        if(compass() > 270 && compass() < 360){
                            while(compass() > 270 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                    }

                    led("amarelo");

                    ResgateDeRé();
                }
                
                //entregar virando na parede
                else if(distancia == 220){
                    print(3, "entregar virando na parede");

                    rot(1000, 90);
                    
                    //ir ate a parede
                    while(sonar(0) > 16){
                        motor(300,300); //?velocidade ajustável (tocar na parede atrás)
                    }
                    
                    //virar
                    rot(1000, 90);

                    //ir reto ate achar zona de resgate
                    while(luz(6) > 10 && sonar(0) > 70){
                        motor(500,500); //?velocidade ajustável (ir reto ate achar zona de resgate)
                    }

                    led("verde");
                    motor(0,0);
            
                    EntregarBolinha();

                    led("amarelo");

                    ResgateDeRé();
                }
            }
           
            //não ta com a bolinha :(
            else{
                motor(-100,-100);
                delay(500);

                goto voltarProcurarBolinha;
            }
    
        }
    }
}

void ResgateDeRé(){
    resgateDeRe = true;
    
    //*bolinhas procurar/pegar/entregar
    voltarProcurarBolinha: 
    led("amarelo");
    
    while(numeroBolinhas < 4){
        
        //ir de costas
        motor(-300,-300); //?velocidade ajustável (ir de costas)

        //verif bolinha no lado
        if(sonar(1) < 225 && sonar(1) > 17){
            
            InitBolinhaNoLado(true);
            {/*
            //ir até a bolinha
            rstTimer();
            while(sonar(2) > 10 && sonar(0) > 5){
                motor(1000,1000); //?velocidade ajustável(ir até a bolinha)

                //bolinha escapo
                if(timer() >= 4000){
                    
                    ledRGB(250, 67, 83);

                    //voltar na parede
                    while(!bc.touch(0)){
                        motor(-1000,-1000);
                    }

                    rot(1000, -90);

                    goto voltarProcurarBolinha;
                }
            }

            */}

            //pegar bolinha   
            if((sonar(0) - sonar(2)) >= 17 && sonar(2) < 40){               
                PegarBolinha();
                led("vermelho");
            }
            else{
                rstTimer();

                while((sonar(0) - sonar(2)) <= 20 || sonar(2) > 30){
                    motor(-1000,1000);

                    if(timer() >= 1500){
                        rstTimer();
                        while((sonar(0) - sonar(2)) <= 20 || sonar(2) > 30){
                            motor(1000,-1000);

                            if(timer() >= 3000){
                                led("vermelho");
                                
                                //correção de giro
                                while(true){
                                    
                                    //Sul
                                    if(compass() > 135 && compass() < 225){
                                        while(compass() > 180 ){
                                            motor(1000,-1000);
                                        }
                                        while(compass() < 180 ){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                    
                                    //Leste
                                    if(compass() > 45 && compass() < 135){
                                        while(compass() > 90 ){
                                            motor(1000,-1000);
                                        }
                                        while(compass() < 90 ){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                    
                                    //Norte
                                    if(compass() < 45 || compass() > 315){
                                        while(compass() > 0.2 && compass() < 45){
                                            motor(1000,-1000);
                                        }
                                        while(compass() > 270 && compass() < 359.8){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                    
                                    //Oeste
                                    if(compass() > 225 && compass() < 315){
                                        while(compass() > 270 ){
                                            motor(1000,-1000);
                                        }
                                        while(compass() < 270 ){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                }

                                //voltar para parede
                                while(!bc.touch(0)){
                                    motor(-1000,-1000);

                                    if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                                        motor(200,200);
                                        delay(1400);
                                        break;
                                    }

                                    if(inclina() > 300){
                                        motor(200,200);
                                        delay(1000);
                                        break;  
                                    }
                                }
                                
                                rot(1000,-90);   

                                goto voltarProcurarBolinha; 
                            }
                        }

                        leftturn = true;
                        rot(1000, -10);
                        break;
                    }
                }
                if(!leftturn){rot(1000, 10);}

                led("verde");
                PegarBolinha();
                led("vermelho");

                //correção de giro
                while(true){
                    
                    //Sul
                    if(compass() > 135 && compass() < 225){
                        while(compass() > 180 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 180 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    
                    //Leste
                    if(compass() > 45 && compass() < 135){
                        while(compass() > 90 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 90 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    
                    //Norte
                    if(compass() < 45 || compass() > 315){
                        while(compass() > 0.2 && compass() < 45){
                            motor(1000,-1000);
                        }
                        while(compass() > 270 && compass() < 359.8){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    
                    //Oeste
                    if(compass() > 225 && compass() < 315){
                        while(compass() > 270 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 270 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                }
                motor(0,0);

            }
            
            //ta com a bolinha
            if(bc.HasVictim()){
                
                //voltar para parede
                for(int a = 150; a <= 200; a++){ //?TESTE
                    motor(-a,-a);
                    delay(1);
                }
                
                while(!bc.touch(0)){ 
                    motor(-200,-200); //?velocidade ajustável (voltar para parede)

                    if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                        motor(200,200);
                        delay(1400);
                        break;
                    }
                }

                //correção de Giro 90°
                rot(1000,-70);
                while(true){
                                            
                    //Sul
                    if(compass() > 180 && compass() < 270){
                        while(compass() > 180 ){
                            motor(1000,-1000);
                        }
                        break;
                    }
                    //Leste
                    if(compass() > 90 && compass() < 180){
                        while(compass() > 90 ){
                            motor(1000,-1000);
                        }
                        break;
                    }
                    //Norte
                    if(compass() > 1 && compass() < 90){
                        while(compass() > 1 ){
                            motor(1000,-1000);
                        }
                        break;
                    }
                    //Oeste
                    if(compass() > 270 && compass() < 360){
                        while(compass() > 270 ){
                            motor(1000,-1000);
                        }
                        break;
                    }
                }

                //ir para resgate
                while(luz(6) > 10 && sonar(0) > 25){
                    motor(300,300); //?velocidade ajustável (ir para resgate)
                } 

                EntregarBolinha();
            
                //correção de giro RETO
                while(true){
                    
                    led("vermelho");
                    
                    //Sul
                    if(compass() > 135 && compass() < 225){
                        while(compass() > 180 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 180 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Leste
                    if(compass() > 45 && compass() < 135){
                        while(compass() > 90 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 90 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Norte
                    if(compass() < 45 || compass() > 315){
                        while(compass() > 1 && compass() < 45){
                            motor(1000,-1000);
                        }
                        while(compass() < 360 && compass() > 315){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Oeste
                    if(compass() > 225 && compass() < 315){
                        while(compass() > 270 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 270 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                }

                goto voltarProcurarBolinha;
            }
            //não ta com a bolinha 
            else{
                
                //voltar para parede
                while(!bc.touch(0)){
                    motor(-1000,-1000);

                    if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                        motor(200,200);
                        delay(1400);
                        break;
                    }
                }
                
                rot(1000,-90);
            }
            
        }

        //bateu na parede costas //Resgate continua
        if((bc.touch(0) && sonar(0) > 242) || sonar(0) > 242){
            
            //correção de giro 90°
            while(true){
        
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    break;
                }
                
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 1 ){
                        motor(1000,-1000);
                    }
                    break;
                }
                
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() < 270){
                        motor(1000,-1000);
                    }
                    while(compass() > 270){
                        motor(1000,-1000);
                    }
                    break;
                }
                
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 180 ){
                        motor(1000, -1000);
                    }
                    break;
                }
            }
            motor(0,0);

            ResgateDeRéContinua();
        }

        if(numeroBolinhas >= 4){
            break;
        }
    }

    SaindoDoResgate();
}

void ResgateDeRéContinua(){
    distancia = 175; 

    //bolinhas procurar/pegar/entregar
    voltarProcurarBolinha:
    led("amarelo");

    while(numeroBolinhas < 4){
        
        //ir de costas
        motor(-300,-300); //?velocidade ajustável (ir de costas)

        //distancia = 180
        if(sonar(0) > 90){
            distancia = 244;
        }
        else{distancia = 180;}

        //verificar bolinha no lado
        if(sonar(1) < distancia && sonar(1) > 17){
                
            InitBolinhaNoLado(true);
            {/*
            //ir até a bolinha
            rstTimer();
            while(sonar(2) > 10 && sonar(0) > 5){
                motor(1000,1000); //?velocidade ajustável(ir até a bolinha)

                //bolinha escapo
                if(timer() >= 4000){
                    
                    ledRGB(250, 67, 83);

                    //voltar na parede
                    while(!bc.touch(0)){
                        motor(-1000,-1000);
                    }

                    rot(1000, -90);

                    goto voltarProcurarBolinha;
                }
            }

            */}

            //pegar bolinha   
            if((sonar(0) - sonar(2)) >= 17 && sonar(2) < 40){
                PegarBolinha();
                led("vermelho");
            }
            else{
                rstTimer();
    
                while((sonar(0) - sonar(2)) <= 20 || sonar(2) > 30){
                    motor(-1000,1000);

                    if(timer() >= 1500){
                        rstTimer();
                        while((sonar(0) - sonar(2)) <= 20 || sonar(2) > 30){
                            motor(1000,-1000);

                            if(timer() >= 3000){
                                led("vermelho");
                                
                                //correção de giro
                                while(true){
                                    
                                    //Sul
                                    if(compass() > 135 && compass() < 225){
                                        while(compass() > 180 ){
                                            motor(1000,-1000);
                                        }
                                        while(compass() < 180 ){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                    
                                    //Leste
                                    if(compass() > 45 && compass() < 135){
                                        while(compass() > 90 ){
                                            motor(1000,-1000);
                                        }
                                        while(compass() < 90 ){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                    
                                    //Norte
                                    if(compass() < 45 || compass() > 315){
                                        while(compass() > 0.2 && compass() < 45){
                                            motor(1000,-1000);
                                        }
                                        while(compass() > 270 && compass() < 359.8){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                    
                                    //Oeste
                                    if(compass() > 225 && compass() < 315){
                                        while(compass() > 270 ){
                                            motor(1000,-1000);
                                        }
                                        while(compass() < 270 ){
                                            motor(-1000,1000);
                                        }
                                        break;
                                    }
                                }

                                //voltar para parede
                                while(!bc.touch(0)){
                                    motor(-1000,-1000);

                                    if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                                        motor(200,200);
                                        delay(1400);
                                        break;
                                    }

                                    if(inclina() > 300){
                                        motor(200,200);
                                        delay(1000);
                                        break;  
                                    }
                                }
                                
                                rot(1000,-90);

                                goto voltarProcurarBolinha;
                            }
                        }

                        leftturn = true;
                        rot(1000, -10);
                        break;
                    }
                }
                if(!leftturn){rot(1000, 10);}

                led("verde");
                PegarBolinha();
                led("vermelho");

                //correção de giro
                while(true){
                    
                    //Sul
                    if(compass() > 135 && compass() < 225){
                        while(compass() > 180 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 180 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    
                    //Leste
                    if(compass() > 45 && compass() < 135){
                        while(compass() > 90 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 90 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    
                    //Norte
                    if(compass() < 45 || compass() > 315){
                        while(compass() > 0.2 && compass() < 45){
                            motor(1000,-1000);
                        }
                        while(compass() > 270 && compass() < 359.8){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    
                    //Oeste
                    if(compass() > 225 && compass() < 315){
                        while(compass() > 270 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 270 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                }
                motor(0,0);
            }

            //ta com a bolinha
            if(bc.HasVictim()){
                
                //de frente para o aberto
                if(sonar(0) > 9000){
                    rot(1000,-90);
                    //correção de giro RETO
                    while(true){
                                
                        led("vermelho");
                        
                        //Sul
                        if(compass() > 135 && compass() < 225){
                            while(compass() > 180 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 180 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Leste
                        if(compass() > 45 && compass() < 135){
                            while(compass() > 90 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 90 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Norte
                        if(compass() < 45 || compass() > 315){
                            while(compass() > 1 && compass() < 45){
                                motor(1000,-1000);
                            }
                            while(compass() < 360 && compass() > 315){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Oeste
                        if(compass() > 225 && compass() < 315){
                            while(compass() > 270 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 270 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                    }
                    motor(0,0);

                    while(true){
                        motor(200,200);

                        //resgate
                        if(luz(6) < 10 && sonar(2) < 10){
                            led("verde");
                            motor(0,0);
                            
                            EntregarBolinha();
                            rot(1000,90);
                            //correção de giro RETO
                            while(true){
                                        
                                led("vermelho");
                                
                                //Sul
                                if(compass() > 135 && compass() < 225){
                                    while(compass() > 180 ){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 180 ){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                                //Leste
                                if(compass() > 45 && compass() < 135){
                                    while(compass() > 90 ){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 90 ){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                                //Norte
                                if(compass() < 45 || compass() > 315){
                                    while(compass() > 1 && compass() < 45){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 360 && compass() > 315){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                                //Oeste
                                if(compass() > 225 && compass() < 315){
                                    while(compass() > 270 ){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 270 ){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                            }
                            
                            break;
                        }
                        //parede
                        if(sonar(0) - sonar(2) <= 20){
                            rot(1000,90);
                            //correção de giro RETO
                            while(true){
                                        
                                led("vermelho");
                                
                                //Sul
                                if(compass() > 135 && compass() < 225){
                                    while(compass() > 180 ){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 180 ){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                                //Leste
                                if(compass() > 45 && compass() < 135){
                                    while(compass() > 90 ){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 90 ){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                                //Norte
                                if(compass() < 45 || compass() > 315){
                                    while(compass() > 1 && compass() < 45){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 360 && compass() > 315){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                                //Oeste
                                if(compass() > 225 && compass() < 315){
                                    while(compass() > 270 ){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 270 ){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                            }
                            
                            //ir para o resgate
                            while(luz(6) > 10 && sonar(0) > 25){
                                motor(500,500); //?velocidade ajustável (ir para o resgate)
                            }
                            
                            led("verde");
                            motor(0,0);

                            EntregarBolinha();

                            rot(1000,90);
                            //correção de giro RETO
                            while(true){
                                        
                                led("vermelho");
                                
                                //Sul
                                if(compass() > 135 && compass() < 225){
                                    while(compass() > 180 ){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 180 ){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                                //Leste
                                if(compass() > 45 && compass() < 135){
                                    while(compass() > 90 ){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 90 ){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                                //Norte
                                if(compass() < 45 || compass() > 315){
                                    while(compass() > 1 && compass() < 45){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 360 && compass() > 315){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                                //Oeste
                                if(compass() > 225 && compass() < 315){
                                    while(compass() > 270 ){
                                        motor(1000,-1000);
                                    }
                                    while(compass() < 270 ){
                                        motor(-1000,1000);
                                    }
                                    break;
                                }
                            }
                            
                            break;
                        }
                    }
                }
                else{
                    while(sonar(0) < 85){
                        motor(-200,-200); //?velocidade ajustável (se afastar da parede)
                    }

                    //correção de Giro 90°
                    rot(1000,-70);
                    while(true){
                                                
                        //Sul
                        if(compass() > 180 && compass() < 270){
                            while(compass() > 180 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                        //Leste
                        if(compass() > 90 && compass() < 180){
                            while(compass() > 90 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                        //Norte
                        if(compass() > 1 && compass() < 90){
                            while(compass() > 1 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                        //Oeste
                        if(compass() > 270 && compass() < 360){
                            while(compass() > 270 ){
                                motor(1000,-1000);
                            }
                            break;
                        }
                    }

                    //ir ate a parede
                    while(sonar(0) > 20){
                        motor(200,200); //?velocidade ajustável (ir ate a parede)
                    }
                
                    rot(1000, 90);
                    //correção de giro RETO
                    while(true){
                                
                        led("vermelho");
                        
                        //Sul
                        if(compass() > 135 && compass() < 225){
                            while(compass() > 180 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 180 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Leste
                        if(compass() > 45 && compass() < 135){
                            while(compass() > 90 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 90 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Norte
                        if(compass() < 45 || compass() > 315){
                            while(compass() > 1 && compass() < 45){
                                motor(1000,-1000);
                            }
                            while(compass() < 360 && compass() > 315){
                                motor(-1000,1000);
                            }
                            break;
                        }
                        //Oeste
                        if(compass() > 225 && compass() < 315){
                            while(compass() > 270 ){
                                motor(1000,-1000);
                            }
                            while(compass() < 270 ){
                                motor(-1000,1000);
                            }
                            break;
                        }
                    }

                    //ir para o resgate
                    while(luz(6) > 10 && sonar(0) > 25){
                        motor(300,300); //?velocidade ajustável (ir para o resgate)
                    } 

                    led("verde");
                    motor(0,0);

                    EntregarBolinha();
                }

                //correção de giro RETO
                while(true){
                    
                    led("vermelho");
                    
                    //Sul
                    if(compass() > 135 && compass() < 225){
                        while(compass() > 180 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 180 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Leste
                    if(compass() > 45 && compass() < 135){
                        while(compass() > 90 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 90 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Norte
                    if(compass() < 45 || compass() > 315){
                        while(compass() > 1 && compass() < 45){
                            motor(1000,-1000);
                        }
                        while(compass() < 360 && compass() > 315){
                            motor(-1000,1000);
                        }
                        break;
                    }
                    //Oeste
                    if(compass() > 225 && compass() < 315){
                        while(compass() > 270 ){
                            motor(1000,-1000);
                        }
                        while(compass() < 270 ){
                            motor(-1000,1000);
                        }
                        break;
                    }
                }

                while(!(bc.touch(0) || sonar(0) > 242)){
                    motor(-1000,-1000); 
                }
                
                led("amarelo");

                //correção de giro 90°
                while(true){
                
                    //Sul
                    if(compass() > 135 && compass() < 225){
                        while(compass() > 90 ){
                            motor(1000,-1000);
                        }
                        break;
                    }
                    
                    //Leste
                    if(compass() > 45 && compass() < 135){
                        while(compass() > 1 ){
                            motor(1000,-1000);
                        }
                        break;
                    }
                    
                    //Norte
                    if(compass() < 45 || compass() > 315){
                        while(compass() < 270){
                            motor(1000,-1000);
                        }
                        while(compass() > 270){
                            motor(1000,-1000);
                        }
                        break;
                    }
                    
                    //Oeste
                    if(compass() > 225 && compass() < 315){
                        while(compass() > 180 ){
                            motor(1000, -1000);
                        }
                        break;
                    }
                }
                motor(0,0);

                goto voltarProcurarBolinha;
            
            }
            //não ta com a bolinha 
            else{
                
                //voltar para parede
                while(!bc.touch(0)){
                    motor(-1000,-1000);

                    if(cor(1) == "VERDE" && cor(5) == "VERDE"){
                        motor(200,200);
                        delay(1400);
                        break;
                    }

                    if(inclina() > 300){
                        motor(200,200);
                        delay(1000);
                        break;  
                    }
                }
                
                rot(1000,-90);
            }  
        }
    
        //bateu na parede costas //Resgate continua
        if((bc.touch(0) && sonar(0) > 242) || sonar(0) > 242){
            
            led("vermelho");

            SaindoDoResgateFaltandoBolinha();
            break;
        }
    }
    
    SaindoDoResgate();
}

void SaindoDoResgate(){
    print(0, "Tenho q ir embora");
    tempoDeGap = 1500;

    //*saida  pos 1
    if(saidaPos1){
        if(resgatePos2){
            print(0, "saidaPos1" + " + " + "resgatePos2");
            
            rot(1000,180);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            while(sonar(1) < 9000){
                motor(300,300);
            }
            led("ciano");

            motor(-300,-300);
            delay(300);
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }
             
        }
        if(resgatePos3){
            print(0, "saidaPos1" + " + " + "resgatePos3");
            
            while(sonar(0) <= 220 ){
                motor(-300,-300);
            }
            rot(1000, 90); 

            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            motor(300,300);
            delay(1500);

            while((!(corR(2) < corG(2) && corB(2) < corG(2) && corG(2) > 60) || !(corR(4) < corG(4) && corB(4) < corG(4) && corG(4) > 60)) && sonar(1) < 9000){
                motor(300,300);
            }
            led("ciano");
            motor(-300,-300);
            delay(200);
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }
        
        }
    }

    //*saida  pos 2
    if(saidaPos2){
        if(resgatePos3){
            print(0, "saidaPos2" + " + " + "resgatePos2");
            
            rot(1000,180);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            while((!(corR(2) < corG(2) && corB(2) < corG(2) && corG(2) > 60) || !(corR(4) < corG(4) && corB(4) < corG(4) && corG(4) > 60)) && sonar(1) < 9000){
                motor(300,300);
            }
            led("ciano");

            motor(300,300);
            delay(500);
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }
        
        }
        if(resgatePos1){
            print(0, "saidaPos2" + " + " + "resgatePos1");
            
            rot(1000,90);

            while(sonar(0) > 25){
                motor(300,300);
            }

            rot(1000,-90);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            while((!(corR(2) < corG(2) && corB(2) < corG(2) && corG(2) > 60) || !(corR(4) < corG(4) && corB(4) < corG(4) && corG(4) > 60)) && sonar(1) < 9000){
                motor(300,300);
            }
            led("ciano");

            motor(300,300);
            delay(500);
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }
        }
    }

    //*saida  pos 3
    if(saidaPos3){
        if(resgatePos1){
            print(0, "saidaPos3" + " + " + "resgatePos1");
            
            while(sonar(0) <= 220 ){
                motor(-300,-300);
            }
            rot(1000, 90); 

            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            motor(300,300);
            delay(1500);

            while((!(corR(2) < corG(2) && corB(2) < corG(2) && corG(2) > 60) || !(corR(4) < corG(4) && corB(4) < corG(4) && corG(4) > 60)) && sonar(1) < 9000){
                motor(300,300);
            }
            led("ciano");
            motor(-300,-300);
            delay(200);
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }
        
        }
        if(resgatePos2){
            print(0, "saidaPos3" + " + " + "resgatePos2");

            rot(1000,90);

            while(sonar(0) > 25){
                motor(300,300);
            }

            rot(1000,-90);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            while((!(corR(2) < corG(2) && corB(2) < corG(2) && corG(2) > 60) || !(corR(4) < corG(4) && corB(4) < corG(4) && corG(4) > 60)) && sonar(1) < 9000){
                motor(300,300);
            }
            led("ciano");
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }
        }
    }

}

void SaindoDoResgateFaltandoBolinha(){
    print(0, "Tenho q ir embora");
    tempoDeGap = 2000;

    //*saida  pos 1
    if(saidaPos1){
        if(resgatePos2){
            print(0, "saidaPos1" + " + " + "resgatePos2");
            while(sonar(0) > 23){
                motor(300,300);
            }
            rot(1000,-90);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            motor(300,300);
            delay(1000);
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }
        }
        if(resgatePos3){
            print(0, "saidaPos1" + " + " + "resgatePos3");
            rot(1000, 150);
            motor(300,300);
            delay(500);
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }
        
        }
    }

    //*saida  pos 2
    if(saidaPos2){
        if(resgatePos3){
            print(0, "saidaPos2" + " + " + "resgatePos3");
            
            while(sonar(0) > 23){
                motor(300,300);
            }
            rot(1000,-90);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            motor(300,300);
            delay(800);
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }

        }
        if(resgatePos1){
            print(0, "saidaPos2" + " + " + "resgatePos1");
            rot(1000,90);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            while(sonar(1) < 9000){
                motor(300,300);
            }
            
            rot(300, 25);
            motor(-300,-300);
            delay(300);
            motor(0,0);
            rot(300, -25);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }
        
        }
    }

    //*saida  pos 3
    if(saidaPos3){
        if(resgatePos1){
            print(0, "saidaPos3" + " + " + "resgatePos1");
            rot(1000, 150);
            motor(300,300);
            delay(500);
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(-1000,1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(1000,-1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }
        }
        if(resgatePos2){
            print(0, "saidaPos3" + " + " + "resgatePos2");
            
            motor(300,300);
            delay(400);
            
            rot(1000,90);
            //correção de giro RETO
            while(true){
                        
                led("vermelho");
                
                //Sul
                if(compass() > 135 && compass() < 225){
                    while(compass() > 180 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 180 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Leste
                if(compass() > 45 && compass() < 135){
                    while(compass() > 90 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 90 ){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Norte
                if(compass() < 45 || compass() > 315){
                    while(compass() > 1 && compass() < 45){
                        motor(1000,-1000);
                    }
                    while(compass() < 360 && compass() > 315){
                        motor(-1000,1000);
                    }
                    break;
                }
                //Oeste
                if(compass() > 225 && compass() < 315){
                    while(compass() > 270 ){
                        motor(1000,-1000);
                    }
                    while(compass() < 270 ){
                        motor(-1000,1000);
                    }
                    break;
                }
            }
            
            motor(300,300);
            delay(2000);

            while(sonar(1) < 9000){
                motor(300,300);
            }
            
            motor(-300,-300);
            delay(300);
            motor(0,0);

            rstTimer();
            while(cor(3) != "PRETO"){
                motor(1000,-1000);

                //Timer para correção
                if(timer() >= 2000){
                    
                    rstTimer();
                    while(cor(1) == "BRANCO" && cor(2) == "BRANCO" && cor(3) == "BRANCO" && cor(4) == "BRANCO" && cor(5) == "BRANCO"){
                        motor(-1000,1000);
                    }
                    break;
                }
            }

            vermelhoSaida = true;

            while(true){
                LineFollow();
                Verde2();
                
            }  
        }
    }

}



//* Funções Secundárias para resgate
void PegarBolinha(){
    motor(0,0);

    //precisa verificar bolinha??
    //if(verifClassif){ClassificarBolinha();}else{pegarBolinha = true;}

    //pegar bolinha
    pegarBolinha = true;
    if(pegarBolinha){

        voltar3:

        //dar espaço para garra
        rstTimer();
        while(sonar(2) <= 25){
            motor(-1000,-1000); //?velocidade ajustável (dar espaço para garra)
            
            if(timer() >= 1000){
                //empurra
                motor(100,100);
                delay(1200);
                
                //pegar
                goto voltar3;
            }
        }
        motor(0,0);

        //descer a garra 
        Descer(79,79);
        //SUBIR: 160 - 132 DESCER: 78 - 40   ATUATOR: 160 - 78   SCOOP: 132 - 40

        //pegar a bolinha
        rstTimer();
        while(!bc.HasVictim()){
            motor(1000,1000);  //?velocidade ajustável (pegar a bolinha)

            if(timer() >= 2500){
                break;
            }
        }

        //impulso
        motor(800,800); //?velocidade ajustável (impulso)
        delay(500);  
        motor(0,0);

        //subir a garra
        motor(10,10);
        Subir(159, 132);
        motor(0,0);
    }
    
    //não pegar bolinha WIP
    {/*
    else{
        while(!bc.touch(0)){
            motor(-500,-500);
        }

        rot(1000, -90);

        if(!resgateDeRe){
            motor(200,200);
            delay(200);
            
        }
        else{
            motor(-200,-200);
            delay(200);
        }
    }   */}
        
}

void EntregarBolinha(){
    led("verde");
    motor(0,0);

    //!dispensar a bolinha
    while(bc.AngleActuator() < 340 ){   
        descerGarra(100, 1);
        print(1, bc.AngleActuator().ToString());
    }

    //!dispensar a bolinha
    /*
    while (bc.HasVictim()){
        descerGarra(150, 10);
        delay(10);
    }*/

    //espera a bolinha cair
    delay(500);

    numeroBolinhas++;
    print(1, "Faltam: " + (4 - numeroBolinhas).ToString()+ " bolinhas");

    //subir garra
    Subir(159, 132);    
}

void PegarBolinhaDeFrente(){
    motor(0,0);
    
    //precisa verificar bolinha??
    if(verifClassif){ClassificarBolinha();}else{pegarBolinha = true;}
    
    //pega a bolinha
    if(pegarBolinha){
        
        voltar2:

        //dar espaço para garra
        rstTimer();
        while(sonar(2) <= 25){
            motor(-300,-300);
            
            if(timer() >= 1500){
                //empurra
                motor(100,100);
                delay(1000);
                
                //pegar
                goto voltar2;
            }
        }
        motor(0,0);

        //descer a garra 
        Descer(79,79);

        //pegar a bolinha
        rstTimer();
        while(!bc.HasVictim()){
            motor(500,500);

            if(timer() >= 2500){
                break;
            }
        }

        //impulso
        motor(800,800);
        delay(800);   

        //subir a garra
        motor(20,20);
        Subir(159, 132);
        motor(0,0); 
    }
    
    //não pega a bolinha
    else{
        rot(1000, -35);

        motor(100,100);
        delay(1200); //ajustavel

        rot(1000, 70);
        rot(1000, -35);
    }  
}

void ClassificarBolinha(){
    
    if(cor(6) == "PRETO"){
        if(numeroBolinhas < 4){
            numeroBolinhas++;
            numeroBolinhasPretas++;
        }
        
        print(1, "PRETA+1" + "  " + "BT:" + "  " + numeroBolinhas + "  " + "BB:" + "  " + numeroBolinhasBrancas + "  " + "BP:" + "  " + numeroBolinhasPretas);
        pegarBolinha = true;
    }

    else if(cor(6) == "BRANCO"){
        
        if(numeroBolinhas < 4){
            numeroBolinhas++;
            numeroBolinhasBrancas++;
        }

        print(1, "BRANCA+1" + "  " + "BT:" + "  " + numeroBolinhas + "  " + "BB:" + "  " + numeroBolinhasBrancas + "  " + "BP:" + "  " + numeroBolinhasPretas);
        pegarBolinha = true;
    }
}

void InitBolinhaNoLado(bool ré){
    

    distanciaAtéBolinha = sonar(1);
    //print(0,distanciaAtéBolinha.ToString());
    led("verde");

    //impulso
    if(!ré){
        //impulso 
        motor(100,100); //?velocidade ajustável (impulso)
        delay(100);
        
        //area de resgate longe
        if(sonar(0) < 165){distancia = 220;} 
    }
    else{
        motor(-300,-300);//?velocidade ajustável (impulso)
        delay(200);
    }
    
    //virar e ficar de frente com a bolinha
    rot(1000,45);
    while(true){
        //Norte => Leste
        if(compass() > 1 && compass() < 90){
            while(compass() < 90){
                motor(-1000,1000);
            }

            break;
        }

        //Leste => Sul
        if(compass() > 90 && compass() < 180){
            while(compass() < 180){
                motor(-1000,1000);
            }

            break;
        }
        //Sul => Oeste
        if(compass() > 180 && compass() < 270){
            while(compass() < 270){
                motor(-1000,1000);
            }

            break;
        }
        //Oeste => Norte
        if(compass() > 270 && compass() < 360){
            while(compass() < 359){
                motor(-1000,1000);
            }

            break;
        }
    }
    motor(0,0);

    //ir para trás
    motor(-300,-300);//?velocidade ajustável (ir para trás)
    delay(200);

    //ir até a bolinha
    tempoDeIda = distanciaAtéBolinha/0.08;
    //print(1,tempoDeIda.ToString());

    rstTimer();
    while(timer() < tempoDeIda){
        motor(1000,1000);

        if(sonar(0) - sonar(2) >= 17 && sonar(2) < 15){
            break;
        } 
    }
    led("azul");

    if(distanciaAtéBolinha > 140){
        motor(300, 300); //?velocidade ajustável(ir para trás)
        delay(300);
    }
    
    motor(0,0);
}