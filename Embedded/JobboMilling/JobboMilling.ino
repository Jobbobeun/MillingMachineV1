#define Version 0
#define Servo1 4
#define Servo2 5
#define Servo3 6
#define Servo4 7
#define Servo5 8
#define Servo6 9
#define StatusLed 2
#define ResetButton 10
#define ExternalError 3

void setup() {
 pinMode(Servo1, Output);
 pinMode(Servo2, Output);
 pinMode(Servo3, Output);
 pinMode(Servo4, Output);
 pinMode(Servo5, Output);
 pinMode(Servo6, Output);
 pinMode(StatusLed, Output);
 pinMode(ExternalError, Output);
 pinMode(ResetButton, Input);

 Serial.begin(250000);
 Serial.print(Version);
 Serial.print(;Connected);
}

void loop() {

}
