# JustGoNsk (Backend) 
## Описание API

### Список событий (опционально с фильтрацией и пагинацией)
  Запрос:

    GET: api/Events/[filter][?offset=2][&count=35]
```json    
        "tags": [
                "шоу (развлечения)",
                "мультимедиа",
                "культура и искусство",
                "красиво"
            ],
      "categories": [
            "exhibition",
            "show"
    ],

       "places": [
            12705,
            302,
            24
    ]
```
 - [ ] *TODO: перенести фильтр в query-параметры*

Ответ:

```json    
    {
        "count": 7,
        "results": [ /* массив event-ов в нашем формате */]
    }
```

### Информация о событии по его ID (в нашей базе)

Запрос:

    GET: api/Events/2

Ответ:

 ```json    
   {
        "id": 2,
        "title": "музей инопланетян",
            "place": {
                "id": 19600
            },
            "description": "<p>Первый в Новосибирске музей НЛО готов принять всех любителей паранормального.</p>\n",
        "place": {
            "id": 3,
            "title": "антикафе Tommy Gun",
		    "address": "ул. Пестеля, д. 27",
		    "coords": {
		        "lat": 59.94270179999986,
		        "lon": 30.34948230000018
		    }
        },
        "categories": [
            "exhibition",
            "show"
        ],
        "tags": [
            "шоу (развлечения)",
            "мультимедиа",
            "картины, живопись, графика",
            "выставки",
            "12+"
        ],
        "images": [
            {
                "image": "https://kudago.com/media/images/event/1a/89/1a89705da915398572538d7fc4a736fc.jpg"
            },
            {
                "image": "https://kudago.com/media/images/event/4d/a8/4da896541296628efeb1cb459393c95f.jpg"
            },
            {
                "image": "https://kudago.com/media/images/event/60/84/6084adda0fca1abb0cb3884c88e35f41.jpg"
            }
        ],
        "dates": [
            {
                "start": "2016-01-03T00:00:00",
                "end": "2016-11-02T00:00:00",
                "has_ended": true
            },
            {
                "start": "2019-06-03T00:00:00",
                "end": "2019-06-04T00:00:00",
                "has_ended": false
            },
            {
                "start": "2019-04-04T00:00:00",
                "end": "2019-04-23T00:00:00",
                "has_ended": false
            }
        ],
        "is_single": false,
        "current": {
            "start": "2019-04-04T00:00:00",
            "end": "2019-04-23T00:00:00",
            "has_ended": false
        },
        "nearest_next": {
            "start": "2019-06-03T00:00:00",
            "end": "2019-06-04T00:00:00",
            "has_ended": false
        }
    }
```
### Добавление события
Запрос:

    POST: api/Events
      
```json    
    {
            "dates": [
                        {
                            "start": "2016-07-01",
                            "end": "2016-10-02"
                        },
                        {
                            "start": "2019-02-12",
                            "end": "2019-05-12"
                        }

            ],
            "title": "выставка «Шедевры импрессионизма. Том 1. Винсент Ван Гог и Эдуард Мане»",
            "place": {
                "id": 32250
            },
            "description": "<p>Галерея  «Мольбеrt» принимает в своих стенах уникальную мультимедийную выставку, где будет представлено более 150 работ двух великих импрессионистов.</p>\n",
            "categories": [
                "exhibition",
                "show"
            ],
            "images": [
                {
                    "image": "https://kudago.com/media/images/event/67/2d/672d98ba53bdeb6ae49becb587708c4b.jpg"
                },
                {
                    "image": "https://kudago.com/media/images/event/3a/26/3a2698aef58d77d0cd1af611954bc0cc.jpg"
                },
                {
                    "image": "https://kudago.com/media/images/event/3f/2f/3f2f0dffcd1b42d64d0a9d1e72469bdb.JPG"
                }
            ],
            "short_title": "Шедевры импрессионизма. Том 1. Винсент Ван Гог и Эдуард Мане",
            "tags": [
                "шоу (развлечения)",
                "мультимедиа",
                "культура и искусство",
                "красиво",
                "новое на сайте",
                "интересное",
                "картины, живопись, графика",
                "выставки",
                "12+"
            ]
}
```
Ответ:  тело такое же как при GET api/Events/{id}, с присвоенным ID из нашей базы, код 201 и ещё если не ошибаюсь в хэдерах указывается адрес где можно получить созданную сущность, т.е. api/Events/{присвоенный ID}. 

### Изменение информации о событии по его ID (в нашей базе)

Запрос:

    PUT: api/Events/456
В теле запроса можно передать всё что передаётся в запросе на добавление события. Если какое-то поле не указано, это значит что его обновлять не надо и оно останется таким как было. Для массивов, чтобы сохранить старые значения надо их все передать вместе с новыми, иначе будет только замена на новые. Для мест принимается в расчёт только ID места,  т.е. нельзя таким косвенным образом изменить информацию о месте.

Ответ: тело такое же как при GET api/Events/{id}

### Удаление события
Запрос:

    DELETE: api/Events/{id}
Ответ: тело такое же как при GET api/Events/{id}

Во всех запросах принимающих ID возвращается 404 если объект не найден.

В запросах на добавление/изменение события, если нам прислали категорию/тэг которой ещё нет в базе, то она будет добавлена в базу.

## API для мест
Эндпоинт `/Places`

Всё то же самое как и для событий, только нет фильтрации (пагинация есть) и модель выглядит так:

```json    
    {
        "id": 27899,
        "title": "антикафе Tommy Gun",
        "address": "ул. Пестеля, д. 27",
        "coords": {
            "lat": 59.94270179999986,
            "lon": 30.34948230000018
        }
    }
```    
Для мест реализовано каскадное удаление, т.е. если вы удалите место с ID=30780 то из базы удалятся все события в которых ID места=30780. При желании можем в будущем настроить по-другому. 



