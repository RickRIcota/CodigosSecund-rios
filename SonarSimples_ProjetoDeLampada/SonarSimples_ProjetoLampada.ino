
//Sonar 1
#define echoPin1 26
#define trigPin1 27

//Sonar 2
#define echoPin2 33
#define trigPin2 25

//Variaveis
int numero = 0;
int DoorWidth = 50; //!CALIBRAR

bool flag1 = 0;
bool flag2 = 0;


void setup()
{
  pinMode(trigPin1, OUTPUT);
  pinMode(echoPin1, INPUT);
  pinMode(trigPin2, OUTPUT);
  pinMode(echoPin2, INPUT);

  Serial.begin(9600);
}


void loop()
{
    //?Condição para Entrada
        if (sonar1() && flag1 == 0)
        {
            flag1 = 1;
            if (flag2 == 0){
                //Adiciona 1 pessoa 
                numero++;
                
                }
        }


    //!Condição para Saida
        if (sonar2() && flag2 == 0)
        {
            flag2 = 1;
            if (flag1 == 0)
            {   
                //Subtrai 1 pessoa 
                numero--;

                //impede de ser negativo
                numero = numero < 0 ? 0 : numero;
            }
        }


    //*Reset de Flag
        if (!sonar1() && !sonar2() && flag1 == 1 && flag2 == 1)
        {
            flag1 = 0, flag2 = 0;
            delay(1000);
        }

    //*Printa valores
    Serial.print("Pessoas: ");
    Serial.print(numero);
    Serial.print("  ");
    Serial.print("A Luz Ta ");
    
    //*Condição de Lampada LIGADA/DESLIGADA
        if (numero > 0)
        {
            Serial.print("LIGADA");
        }
        else
        {
            Serial.print("DESLIGADA");
        }
}


bool sonar1()
{
    //leitura do Sonar1
    long duration;
    int distance;
    bool Enter = false;

    digitalWrite(trigPin1, LOW);
    delayMicroseconds(2);

    digitalWrite(trigPin1, HIGH);
    delayMicroseconds(10);
    digitalWrite(trigPin1, LOW);

    duration = pulseIn(echoPin1, HIGH);

    distance = duration * 0.034 / 2;
    

    Serial.print(distance);
    Serial.print("  ");

    //Condições
        
        //Passou pelo sonar
        if (distance <= DoorWidth)
        {
            Enter = true;
            return (Enter);
        }

        //Não passou pelo sonar
        else
        {
            Enter = false;
            return (Enter);
        }
}

bool sonar2()
{

    //leitura do Sonar2
    long duration;
    int distance;
    bool Enter = false;

    digitalWrite(trigPin2, LOW);
    delayMicroseconds(2);

    digitalWrite(trigPin2, HIGH);
    delayMicroseconds(10);
    digitalWrite(trigPin2, LOW);

    duration = pulseIn(echoPin2, HIGH);

    distance = duration * 0.034 / 2;


    Serial.println(distance);
    
    //Condições

        //Passou pelo sonar
        if (distance <= DoorWidth)
        {
            Enter = true;
            return (Enter);
        }

        //Não passou pelo sonar
        else
        {
            Enter = false;
            return (Enter);
        }
}
