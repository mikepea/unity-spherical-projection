﻿#pragma strict

var speed = 5.0;

function Start () {

}

function Update () {
  var x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
  var y = Input.GetAxis("Vertical") * Time.deltaTime * speed;
  transform.Translate(x, y, 0);
  //transform.forward = Vector3.zero;
  //transform.LookAt(Vector3.zero, Vector3.left);
}