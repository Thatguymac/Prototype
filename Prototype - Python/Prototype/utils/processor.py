# PROCESSOR

import string
import nltk

from nltk.tokenize import word_tokenize, MWETokenizer
from nltk.corpus import stopwords
from nltk.stem import WordNetLemmatizer
from nltk import pos_tag
from nltk.corpus.reader.wordnet import VERB, NOUN, ADV, ADJ

class Processor:
    def __init__(self):
        nltk.download('stopwords', quiet=True)
        nltk.download('averaged_perceptron_tagger', quiet=True)
        nltk.download('wordnet', quiet=True)
        nltk.download('omw-1.4', quiet=True)
         

        self.stopwords_to_keep = ['my', 'me', 'you', 'i', 'and', 'under', 'she']
        self.english_stopwords = set(stopwords.words('english'))
        
        for word in self.stopwords_to_keep:
            self.english_stopwords.discard(word)  # Remove these words from the stopwords set
            
        self.phrases = [('give', 'me'), ('thank', 'you'), ('how', 'many'), ('my', 'brother')]
        self.tokenizer = MWETokenizer()
        for phrase in self.phrases:
            self.tokenizer.add_mwe(phrase)
        
        self.lemmatizer = WordNetLemmatizer()
        
    def get_wordnet_pos(self, word, tag):
        if tag.startswith('J'):
            return ADJ  # Adjective
        elif tag.startswith('V'):
            return VERB  # Verb
        elif tag.startswith('N'):
            return NOUN  # Noun
        elif tag.startswith('R'):
            return ADV  # Adverb
        else:
            return NOUN  # Default to Noun if unknown tag
        
    def preprocess_text(self, text):
        processed_text = text.translate(str.maketrans('', '', string.punctuation)).lower()
        words = self.tokenizer.tokenize(word_tokenize(processed_text))
        filtered_words = [word for word in words if word not in self.english_stopwords]
        pos_tags = pos_tag(filtered_words)
        lemmatized_words = [self.lemmatizer.lemmatize(word, pos=self.get_wordnet_pos(word, tag)) for word, tag in pos_tags]

        return lemmatized_words