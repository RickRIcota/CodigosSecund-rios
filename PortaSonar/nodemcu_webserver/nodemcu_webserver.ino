#include <WiFi.h>
#include <WiFiClient.h>
#include <WebServer.h>
#include <ESPmDNS.h>

const char* ssid = "2.4Ghz_Sagitarios";
const char* password = "rigel123";

WebServer server(80);

const int led = 2;

//////////////////////////
#define echoPin1 26
#define trigPin1 27

#define echoPin2 33
#define trigPin2 25

int numero = 0;
int DoorWidth = 50; //!CALIBRAR

bool flag1 = 0, flag2 = 0;

void handleRoot() {

  String textoHTML;

  textoHTML = "Pessoas na sala: ";
  textoHTML += numero;

  if(numero > 0){
    textoHTML += " => <b>LUZ LIGADA</b>";
  }
  else{
    textoHTML += " => <b>LUZ DESLIGADA</b>";
  }
   
  server.send(200, "text/html", textoHTML);
}

void handleNotFound(){
  digitalWrite(led, 1);

  String message = "File Not Found\n\n";

  message += "URI: ";
  message += server.uri();
  message += "\nMethod: ";
  message += (server.method() == HTTP_GET)?"GET":"POST";
  message += "\nArguments: ";
  message += server.args();
  message += "\n";
  for (uint8_t i=0; i<server.args(); i++){
    message += " " + server.argName(i) + ": " + server.arg(i) + "\n";
  }
  server.send(404, "text/plain", message);
  digitalWrite(led, 0);
}

void setup(){
  pinMode(led, OUTPUT);
  digitalWrite(led, 0);
  Serial.begin(115200);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  Serial.println("");

  // Wait for connection
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());

  if (MDNS.begin("esp8266")) {
    Serial.println("MDNS responder started");
  }

  server.on("/", handleRoot);

  server.on("/inline", [](){
    server.send(200, "text/plain", "this works as well");
  });

  server.onNotFound(handleNotFound);

  server.begin();
  Serial.println("HTTP server started");

  pinMode(trigPin1, OUTPUT);
  pinMode(echoPin1, INPUT);
  pinMode(trigPin2, OUTPUT);
  pinMode(echoPin2, INPUT);
}

void loop(){

  server.handleClient();
  entranceCount();

}

void entranceCount(){
  //?ENTRO/////////////////////////
  if (sonar1() && flag1 == 0){
    flag1 = 1;

    if (flag2 == 0){
      numero++;
    }
  }

  //!SAIU/////////////////////////
  if (sonar2() && flag2 == 0){
    flag2 = 1;

    if (flag1 == 0){
      numero--;
      numero = numero < 0 ? 0 : numero;
    }
  }

  if (!sonar1() && !sonar2() && flag1 == 1 && flag2 == 1){
    flag1 = 0, flag2 = 0;
    delay(500);//1000
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