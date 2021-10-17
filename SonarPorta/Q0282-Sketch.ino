#include <WiFi.h>
#include <WiFiClient.h>
#include <WebServer.h>
#include <ESPmDNS.h>

#include <PubSubClient.h>

//////////////////////////
#define echoPin1 26
#define trigPin1 27

#define echoPin2 33
#define trigPin2 25

int numero = 0;
int DoorWidth = 50; //!CALIBRAR

bool flag1 = 0, flag2 = 0;

//WiFi
const char* SSID = "2.4Ghz_Sagitarios";                // SSID / nome da rede WiFi que deseja se conectar
const char* PASSWORD = "rigel123";   // Senha da rede WiFi que deseja se conectar
WiFiClient wifiClient;                        
 
//MQTT Server
const char* BROKER_MQTT = "mqtt.eclipseprojects.io"; //URL do broker MQTT que se deseja utilizar
int BROKER_PORT = 1883;                      // Porta do Broker MQTT

#define ID_MQTT  "EspRick"            //Informe um ID unico e seu. Caso sejam usados IDs repetidos a ultima conexão irá sobrepor a anterior. 
#define TOPIC_PUBLISH "YadaaBotao1"    //Informe um Tópico único. Caso sejam usados tópicos em duplicidade, o último irá eliminar o anterior.
PubSubClient MQTT(wifiClient);        // Instancia o Cliente MQTT passando o objeto espClient

//Declaração das Funções
void mantemConexoes();  //Garante que as conexoes com WiFi e MQTT Broker se mantenham ativas
void conectaWiFi();     //Faz conexão com WiFi
void conectaMQTT();     //Faz conexão com Broker MQTT
void enviaPacote();     //

void setup() {
  pinMode(trigPin1, OUTPUT);
  pinMode(echoPin1, INPUT);
  pinMode(trigPin2, OUTPUT);
  pinMode(echoPin2, INPUT);       

  Serial.begin(115200);

  conectaWiFi();
  MQTT.setServer(BROKER_MQTT, BROKER_PORT);   
}

void loop() {
  countSonar();
  
  mantemConexoes();
  enviaValores();
  MQTT.loop();
}

void countSonar(){
  
  if (sonar1() && flag1 == false){ //condição de entrada
    flag1 = true;

    if (flag2 == false){
      numero++;
      Serial.println(numero);
    }
  }

  if (sonar2() && flag2 == false){ //condição de saida
    flag2 = true;

    if (flag1 == false){
      numero--;
      numero = numero < 0 ? 0 : numero;
      Serial.println(numero);
    }
  }

  if (!sonar1() && !sonar2() && flag1 == true && flag2 == true){
    flag1 = false, flag2 = false;
    delay(500); //1000
  }
}

bool sonar1(){

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
    
    delay(50);

  //Serial.print(distance);
  //Serial.print("  ");
    
  if (distance <= DoorWidth){
    Enter = true;
    return (Enter);
        
  }
  else{
    Enter = false;
    return (Enter);
  }
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

  delay(50);

  //Serial.println(distance);
  
  if (distance <= DoorWidth){
    Enter = true;
    return (Enter);
      
  }
  else{
    Enter = false;
    return (Enter);
  }
}


void mantemConexoes() {
  if (!MQTT.connected()) {
    conectaMQTT(); 
  }
  
  conectaWiFi(); //se não há conexão com o WiFI, a conexão é refeita
}

void conectaWiFi() {

  if (WiFi.status() == WL_CONNECTED) {
    return;
  }
        
  Serial.print("Conectando-se na rede: ");
  Serial.print(SSID);
  Serial.println("  Aguarde!");

  WiFi.begin(SSID, PASSWORD); // Conecta na rede WI-FI  
  while (WiFi.status() != WL_CONNECTED) {
    delay(100);
    Serial.print(".");
  }
  
  Serial.println();
  Serial.print("Conectado com sucesso, na rede: ");
  Serial.print(SSID);  
  Serial.print("  IP obtido: ");
  Serial.println(WiFi.localIP()); 
}

void conectaMQTT() { 
  while (!MQTT.connected()) {
    Serial.print("Conectando ao Broker MQTT: ");
    Serial.println(BROKER_MQTT);
    if (MQTT.connect(ID_MQTT)) {
      Serial.println("Conectado ao Broker com sucesso!");
    } 
    else {
      Serial.println("Noo foi possivel se conectar ao broker.");
      Serial.println("Nova tentatica de conexao em 5s");
      delay(5000);
      
  }
}
}

void enviaValores() {

  if (numero > 0) {
  
    //Lampada ligada     
    MQTT.publish(TOPIC_PUBLISH, "1");
    Serial.println("Lampada ligada ");
  } else if (numero == 0) {
  
    //Lampada desligada 
    MQTT.publish(TOPIC_PUBLISH, "0");
    Serial.println("Lampada desligada ");
  }
}
