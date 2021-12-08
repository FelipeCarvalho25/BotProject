#!/usr/bin/env python
# -*- coding: utf-8 -*-
import nltk
from nltk.stem import WordNetLemmatizer
import pickle
import numpy as np
from tensorflow.keras.models import load_model
import json
import random
from dialog import *


class ChatBot:

    ERROR_THRESHOLD = 0.8

    MODE_NORMAL = 0
    MODE_DIALOG = 1

    def __init__(self):
        self.lemmatizer = WordNetLemmatizer()

        self.model = load_model('chatbot_model.h5')

        self.intents = json.loads(open('intents.json').read())
        self.words = pickle.load(open('words.pkl', 'rb'))
        self.classes = pickle.load(open('classes.pkl', 'rb'))

        self.mode = self.MODE_NORMAL
        self.dialogs = Talk()
        ticket_dialog = Dialog('ticket')
        ticket_dialog.add_state('Que pena, parece que não consegui te ajudar.\nCerto, escreva com o máximo de detalhes a sua dúvida:')
        ticket_dialog.add_state('Certo, agora informe seu nome:')
        ticket_dialog.add_state('Agora preciso do seu e-mail:')
        ticket_dialog.add_state('Agora informe seu problema ou solicitação:')
        ticket_dialog.add_state('')
        register_dialog = Dialog('register')
        register_dialog.add_state('Certo, qual dado você gostaria de mudar?')
        register_dialog.add_state('Qual o valor você quer atribuir a este dado?')
        register_dialog.add_state('')
        blackout_dialog = Dialog('blackout')
        blackout_dialog.add_state('Certo, agora informe seu nome:')
        blackout_dialog.add_state('Qual o endereço onde ocorreu a queda de energia?')
        blackout_dialog.add_state('Qual o horário que ocorreu a queda de energia?')
        blackout_dialog.add_state('Informe um e-mail para contato:')
        blackout_dialog.add_state('')
        religate_dialog = Dialog('religate')
        religate_dialog.add_state('Certo, informe seu nome:')
        religate_dialog.add_state('Qual o endereço em que deseja religar?')
        religate_dialog.add_state('Informe um horário de preferência para visita técnica:')
        religate_dialog.add_state('Informe um e-mail para contato:')
        religate_dialog.add_state('')
        self.dialogs.add_dialog(ticket_dialog)
        self.dialogs.add_dialog(register_dialog)
        self.dialogs.add_dialog(blackout_dialog)
        self.dialogs.add_dialog(religate_dialog)
    def clean_up_sentence(self, sentence):
        sentence_words = nltk.word_tokenize(sentence)
        sentence_words = [self.lemmatizer.lemmatize(word.lower()) for word in sentence_words]
        return sentence_words

    def bow(self, sentence, words, show_details=True):
        sentence_words = self.clean_up_sentence(sentence)
        bag = [0] * len(words)
        for s in sentence_words:
            for i, w in enumerate(words):
                if w == s:
                    bag[i] = 1
                    if show_details:
                        print("found in bag: %s" % w)
        return np.array(bag)

    def predict_class(self, sentence, model):
        p = self.bow(sentence, self.words, show_details=False)
        res = model.predict(np.array([p]))[0]
        results = [[i, r] for i, r in enumerate(res) if r > self.ERROR_THRESHOLD]
        results.sort(key=lambda x: x[1], reverse=True)
        return_list = []
        for r in results:
            return_list.append({"intent": self.classes[r[0]], "probability": str(r[1])})
        return return_list

    def get_response(self, ints, intents_json, msg):
        tag = ints[0]['intent']
        list_of_intents = intents_json['intents']
        result = None
        if self.mode == self.MODE_NORMAL:
            for i in list_of_intents:
                if i['tag'] == tag:
                    if tag in ('ticket', 'register', 'blackout', 'religate'):
                        self.mode = self.MODE_DIALOG
                        self.dialogs.set_dialog(tag)
                    else:
                        result = random.choice(i['responses'])
                    break
        if self.mode == self.MODE_DIALOG:
            result = self.dialogs.current_dialog.current(msg).msg
            if self.dialogs.current_dialog.next():
                self.mode = self.MODE_NORMAL
                if self.dialogs.current_dialog.name == 'ticket':
                    result += 'Sr(a) ' + \
                              self.dialogs.current_dialog.states[1].var + \
                              ', seu ticket foi criado com sucesso. Enviaremos um e-mail para "' + \
                              self.dialogs.current_dialog.states[2].var + '" assim que tivermos uma resposta.'
                elif self.dialogs.current_dialog.name == 'register':
                    result += 'Certo, o dado ' + \
                              self.dialogs.current_dialog.states[0].var + \
                              'foi atualizado para "' + \
                              self.dialogs.current_dialog.states[1].var + '" .'
                elif self.dialogs.current_dialog.name == 'blackout':
                    result += 'Sr(a) ' + \
                              self.dialogs.current_dialog.states[0].var + \
                              ', seu incidente foi cadastrado, nossa equipe estará trabalhando para reestabelecer a energia e avisaremos no e-mail"' + \
                              self.dialogs.current_dialog.states[3].var + '" assim que tivermos uma resposta.'
                elif self.dialogs.current_dialog.name == 'religate':
                    result += 'Sr(a) ' + \
                              self.dialogs.current_dialog.states[0].var + \
                              ', sua solicitação foi adicionada, estaremos agendando uma visita técnica e avisaremos no e-mail"' + \
                              self.dialogs.current_dialog.states[3].var + '" assim que tivermos uma resposta.'
        return result

    def chatbot_response(self, msg):
        ints = self.predict_class(msg, self.model)
        res = self.get_response(ints, self.intents, msg)
        return res
