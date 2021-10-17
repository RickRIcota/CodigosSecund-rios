
//////////////////////////
#define echoPin1 12
#define trigPin1 11

#define echoPin2 10
#define trigPin2 9

int numero = 0;
int DoorWidth = 50; //!CALIBRAR
int distanceSonar1 = 0;
int distanceSonar2 = 0;

long duration;
int distance;
bool Enter = false;

bool flag1 = 0, flag2 = 0;

void setup() {
  pinMode(trigPin1, OUTPUT);
  pinMode(echoPin1, INPUT);
  pinMode(trigPin2, OUTPUT);
  pinMode(echoPin2, INPUT);       

  Serial.begin(9600);

}

void loop() {

  digitalWrite(trigPin1, LOW);
  delayMicroseconds(2);

  digitalWrite(trigPin1, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin1, LOW);

  duration = pulseIn(echoPin1, HIGH);

  distance = duration * 0.034 / 2;
 
  Serial.println(distance);
}

/*
void sonartest(){
  if (sonar1() && flag1 == false){ //condição de entrada
    flag1 = true;

    if (flag2 == false){
      numero++;
    }
  }

  if (sonar2() && flag2 == false){ //condição de saida
    flag2 = true;

    if (flag1 == false){
      numero--;
      numero = numero < 0 ? 0 : numero;
    }
  }

  if (!sonar1() && !sonar2() && flag1 == true && flag2 == true){
    flag1 = false, flag2 = false;
    delay(500); //1000
  }

  //SonarDebug();
}

void SonarDebug(){
  Serial.print("Distancias (cm):  ");
  Serial.print(distanceSonar1);
  Serial.print(" || ");
  Serial.print(distanceSonar2);
  Serial.println();
}


bool sonar2(){
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

  distanceSonar2 = distance;
  
  if (distance <= DoorWidth){
    Enter = true;
    return (Enter);
      
  }
  else{
    Enter = false;
    return (Enter);
  }
}*/

