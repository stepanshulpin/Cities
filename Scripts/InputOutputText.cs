using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

public class InputOutputText : MonoBehaviour
{

    InputField input;
    InputField.SubmitEvent se;
    public Image errorImage;
    public Text output;
    public Text outputComputer;
    public Text error;
    XmlDocument xmlDoc = new XmlDocument();
    List<string> usingCities = new List<string>();

    // Use this for initialization
    void Start()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("cities", typeof(TextAsset));
        xmlDoc.Load(Application.dataPath + "/Cities.xml");
        input = gameObject.GetComponent<InputField>();
        se = new InputField.SubmitEvent();
        se.AddListener(SubmitInput);
        input.onEndEdit = se;
    }

    void parseXmlFile()
    {
        //string total = "";

       

        //string xmlPathPattern = "//rocid/city";
        /*XmlNodeList myNodeList = xmlDoc.GetElementsByTagName("city");
        foreach (XmlNode node in myNodeList)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {

                switch (childNode.Name)
                {
                    case "name":
                        output.text += childNode.InnerText + "\n"; break;
                }
            }
        }*/
        
    }

    void SubmitInput(string arg0)
    {
        //Формальная проверка на пустоту ввода
        if (arg0 != "")
        {
            
            string playerText = "";
            char lastLetterComputer;
            char lastLetterPlayer;
            playerText += Char.ToUpper(arg0[0]) + arg0.Substring(1);
            //Проверка на первый раз или нет
            if (outputComputer.text != "")
            {
                if ((Char.ToUpper(outputComputer.text[outputComputer.text.Length - 2]) == 'Ь') ||
                    (Char.ToUpper(outputComputer.text[outputComputer.text.Length - 2]) == 'Ъ') ||
                    (Char.ToUpper(outputComputer.text[outputComputer.text.Length - 2]) == 'Ы') ||
                    (Char.ToUpper(outputComputer.text[outputComputer.text.Length - 2]) == 'Ё'))
                    lastLetterComputer = Char.ToUpper(outputComputer.text[outputComputer.text.Length - 3]);
                else { lastLetterComputer = Char.ToUpper(outputComputer.text[outputComputer.text.Length - 2]); }
                //Last буква компа != первой введеной
                if (lastLetterComputer != playerText[0])
                {
                    error.text = "Город должен начинаться на букву \"" + lastLetterComputer + "\"";
                    error.GetComponent<Animation>().Play();
                    errorImage.GetComponent<Animation>().Play();
                    input.text = "";
                }
                //Не первый раз, буква нужная
                else {
                    string path = "descendant::city[name = \"" + playerText + "\"]";
                    //Проверка введеного города на наличие в базе
                    if (xmlDoc.SelectSingleNode(path) != null)
                    {
                        error.text = "";
                        //Использовался ли ранее
                        int usingNamePlayer = 0;
                        //Цикл по использованным, пытаясь найти
                        foreach (string city in usingCities)
                        {
                            if (city == playerText)
                            {
                                usingNamePlayer++;
                            }
                        }
                        //Не нашли
                        if (usingNamePlayer == 0)
                        {
                            //Добавляем в список использованных
                            usingCities.Add(playerText);
                            //Вывод в окно игрока
                            output.text += playerText + "\n";
                            input.text = "";
                            if ((Char.ToUpper(output.text[output.text.Length - 2]) == 'Ь') ||
                                (Char.ToUpper(output.text[output.text.Length - 2]) == 'Ъ') ||
                                (Char.ToUpper(output.text[output.text.Length - 2]) == 'Ы') ||
                                (Char.ToUpper(output.text[output.text.Length - 2]) == 'Ё'))
                                lastLetterPlayer = Char.ToUpper(output.text[output.text.Length - 3]);
                            else { lastLetterPlayer = Char.ToUpper(output.text[output.text.Length - 2]); }
                            //Ход компьютера
                            string computerText = "";
                            int f = 0;
                            //Среди всех городов
                            XmlNodeList myNodeList = xmlDoc.GetElementsByTagName("city");
                            foreach (XmlNode node in myNodeList)
                            {
                                foreach (XmlNode childNode in node.ChildNodes)
                                {
                                    //Поиск по свойству "name", чтоб совпала первая буква
                                    if ((childNode.Name == "name") && (childNode.InnerText[0] == lastLetterPlayer))
                                    {
                                        //Поиск найденного города в списке использованных
                                        int usingName = 0;
                                        foreach (string city in usingCities)
                                        {
                                            if (city == childNode.InnerText)
                                            {
                                                usingName++;
                                            }
                                        }
                                        //Для не использованного сохраняем, выходим из цикла
                                        if (usingName == 0) { computerText = childNode.InnerText; f++; break; }
                                    }
                                }
                                if (f != 0) break;                               
                            }
                            //Нашелся ли город на заданную букву еще не использованный?
                            if (computerText == "")
                            {
                                error.color = new Color(1, 0, 0, 1);
                                error.text = "Вы выиграли";
                                errorImage.color = new Color(1F, 1F, 1F, 1F);
                                input.gameObject.SetActive(false);
                            }
                            else
                            {
                                //Если нашелся, добавляем в список использованных, в поле вывода комп
                                error.text = "";
                                usingCities.Add(computerText);
                                outputComputer.text += computerText + "\n";
                            }
                        }
                        //Нашли среди использованных
                        else
                        {
                            error.text = "Город " + playerText + " уже использовался";
                            error.GetComponent<Animation>().Play();
                            errorImage.GetComponent<Animation>().Play();
                            input.text = "";
                        }
                        XDocument doc = XDocument.Load(Application.dataPath + "/Cities.xml");
                        XElement xel = doc.Element("Cities")
                            .Elements("city")
                            .Where(x => x.Element("name").Value == playerText)
                           .SingleOrDefault();
                        xel.Element("rating").Value = (Int32.Parse(xel.Element("rating").Value) + 1).ToString();
                        doc.Save(Application.dataPath + "/Cities.xml");

                        var xml = doc.Element("Cities")
                        .Elements("city")
                        .OrderByDescending(s => Int32.Parse(s.Element("rating").Value));

                        XDocument xdoc = new XDocument(new XElement("Cities", xml));
                        xdoc.Save(Application.dataPath + "/Cities.xml");
                    }
                    //Не нашли введеный город в базе
                    else
                    {
                        error.text = "Города " + playerText + " не существует!";
                        error.GetComponent<Animation>().Play();
                        errorImage.GetComponent<Animation>().Play();
                        input.text = "";
                    }                    
                }
            }
            //Первый раз
            else
            {
                string path = "descendant::city[name = \"" + playerText + "\"]";
                //Поиск введеного города в базе
                if (xmlDoc.SelectSingleNode(path) != null)
                {
                    error.text = "";
                    //Добавляем в базу
                    usingCities.Add(playerText);
                    //Добавляем в окно вывода пользователя
                    output.text += playerText + "\n";
                    input.text = "";
                    if ((Char.ToUpper(output.text[output.text.Length - 2]) == 'Ь') ||
                        (Char.ToUpper(output.text[output.text.Length - 2]) == 'Ъ') ||
                        (Char.ToUpper(output.text[output.text.Length - 2]) == 'Ы') ||
                        (Char.ToUpper(output.text[output.text.Length - 2]) == 'Ё'))
                        lastLetterPlayer = Char.ToUpper(output.text[output.text.Length - 3]);
                    else { lastLetterPlayer = Char.ToUpper(output.text[output.text.Length - 2]); }
                    //Ход компьютера
                    string computerText = "";
                    int f = 0;
                    //Поиск по тегу город
                    XmlNodeList myNodeList = xmlDoc.GetElementsByTagName("city");
                    foreach (XmlNode node in myNodeList)
                    {
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            //Среди полей "name" ищем которые начинаются с нужной буквы
                            if ((childNode.Name == "name") && (childNode.InnerText[0] == lastLetterPlayer))
                            {
                                int usingName = 0;
                                //Проверка на использование ранее
                                foreach (string city in usingCities)
                                {
                                    if (city == childNode.InnerText)
                                    {
                                        usingName++;
                                    }
                                }
                                //если не использовано запоминаем выходим из цикла
                                if (usingName == 0) { computerText = childNode.InnerText; f++; break; }
                            }
                        }
                        if (f != 0) break;
                    }
                    //Если не найдено
                    if (computerText == "")
                    {
                        error.color = new Color(1, 0, 0, 1);
                        error.text = "Вы выиграли";
                        errorImage.color = new Color(1F, 1F, 1F, 1F);
                        input.gameObject.SetActive(false);
                    }
                    //Если найдено добавить в поле вывода компьютера и в список использованных
                    else
                    {
                        error.text = "";
                        usingCities.Add(computerText);
                        outputComputer.text += computerText + "\n";
                    }
                    XDocument doc = XDocument.Load(Application.dataPath + "/Cities.xml");
                    XElement xel = doc.Element("Cities")
                        .Elements("city")
                        .Where(x => x.Element("name").Value == playerText)
                       .SingleOrDefault();
                    xel.Element("rating").Value = (Int32.Parse(xel.Element("rating").Value) + 1).ToString();
                    doc.Save(Application.dataPath + "/Cities.xml");
                    var xml = doc.Element("Cities")
                        .Elements("city")
                        .OrderByDescending(s => Int32.Parse(s.Element("rating").Value));

                    XDocument xdoc = new XDocument(new XElement("Cities", xml));
                    xdoc.Save(Application.dataPath + "/Cities.xml");
                }
                //Введеного первый раз города нет в базе
                else
                {
                    error.text = "Города " + playerText + " не существует!";
                    error.GetComponent<Animation>().Play();
                    errorImage.GetComponent<Animation>().Play();
                    input.text = "";
                }
            }
        }
    }
}
