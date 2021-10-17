/*
 * uMQTTBroker demo for Arduino (C++-style)
 * 
 * The program defines a custom broker class with callbacks, 
 * starts it, subscribes locally to anything, and publishs a topic every second.
 * Try to connect from a remote client and publish something - the console will show this as well.
 */

#include <ESP8266WiFi.h>
#include "uMQTTBroker.h"

/*
 * Your WiFi config here
 */
char ssid[] = "1 Ticket Okita";     // your network SSID (name)
char pass[] = "J@ckd@wM0rr1g@n"; // your network password
bool WiFiAP = false;      // Do yo want the ESP as AP?

WiFiServer server(80);
char customtext[100];
String header;
//String outputState = "Submit";

/*
 * Custom broker class with overwritten callback functions
 */
class myMQTTBroker: public uMQTTBroker
{
public:
    virtual bool onConnect(IPAddress addr, uint16_t client_count) {
      Serial.println(addr.toString()+" connected");
      return true;
    }
    
    
    virtual void onData(String topic, const char *data, uint32_t length) {
      char data_str[length+1];
      os_memcpy(data_str, data, length);
      data_str[length] = '\0';
      
      //Serial.println("received topic '"+topic+"' with data '"+(String)data_str+"'");
    }
};

myMQTTBroker myBroker;

/*
 * WiFi init stuff
 */
void startWiFiClient()
{
  Serial.println("Connecting to "+(String)ssid);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, pass);
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  
  Serial.println("WiFi connected");
  Serial.println("IP address: " + WiFi.localIP().toString());

  server.begin();
  Serial.println("Server started");
}

void startWiFiAP()
{
  WiFi.mode(WIFI_AP);
  WiFi.softAP(ssid, pass);
  Serial.println("AP started");
  Serial.println("IP address: " + WiFi.softAPIP().toString());
}


void setup()
{
  Serial.begin(115200);
  Serial.println();
  Serial.println();

  // Start WiFi
  if (WiFiAP)
    startWiFiAP();
  else
    startWiFiClient();

  // Start the broker
  Serial.println("Starting MQTT broker");
  myBroker.init();

/*
 * Subscribe to anything
 */
 //Serial.print("Publish message: ");
    //Serial.println(customtext);
 //myBroker.publish("customtext"); myBroker.publish("broker/counter", (String)counter++);
 // myBroker.subscribe("customtext");
}

//int counter = 0;

void loop()
{
  
  WiFiClient client = server.available();
  if (client) {                             // If a new client connects,
    Serial.println("New Client.");          // print a message out in the serial port
    String currentLine = "";                // make a String to hold incoming data from the client
    while (client.connected()) {            // loop while the client's connected
      if (client.available()) {             // if there's bytes to read from the client,
        char c = client.read();             // read a byte, then
        Serial.write(c);                    // print it out the serial monitor
        header += c;
        if (c == '\n') {                    // if the byte is a newline character
          // if the current line is blank, you got two newline characters in a row.
          // that's the end of the client HTTP request, so send a response:
          if (currentLine.length() == 0) {
            // HTTP headers always start with a response code (e.g. HTTP/1.1 200 OK)
            // and a content-type so the client knows what's coming, then a blank line:
            client.println("HTTP/1.1 200 OK");
            client.println("Content-type:text/html");
            client.println("Connection: close");
            client.println();
            
            // turns the GPIOs on and off
            //if (header.indexOf("GET customtext") >= 0) {
        //Serial.println("text submitted");
        //outputState = "submit";
        //}

            // Display the HTML web page
            
           client.println("<!DOCTYPE html> <html> <body> ");
           client.println("<h1>Welcome to the ESP8266 LED Matrix</h1> ");
           client.println("<p>Input Your Text</p>");
           client.println("<form action=\"/\" method=\"get\">");
          client.println("<INPUT type=\"text\" size=\"50\" name=\"customtext\"<br><br>");
          client.println("<INPUT type=\"submit\" value=\"Send\">");
          client.println("</form>");
           
           client.println("</body> </html> \n");
           
         
          
            // The HTTP response ends with another blank line
            client.println();
            // Break out of the while loop
            break;
          } else { // if you got a newline, then clear currentLine
            currentLine = "";
          }
        } else if (c != '\r') {  // if you got anything else but a carriage return character,
          currentLine += c;      // add it to the end of the currentLine
        }
      }
    }
    // Clear the header variable
    header = "";
    // Close the connection
    client.stop();
    Serial.println("Client disconnected.");
    Serial.println("");
  }


  //myBroker.publish("Get customtext ", (String)counter);
//Serial.print("Publish message: ");
    Serial.println(customtext);
    myBroker.publish("outTopic", customtext);
  // wait a second
  delay(1000);
}
