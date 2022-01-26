# -*- coding: utf-8 -*-

import numpy as np
import tensorflow as tf
from tensorflow.keras import layers
from tensorflow.keras.models import load_model

from data_loader import load_vectorization_from_disk
from positional_embedding import PositionalEmbedding
from transformer_decoder import TransformerDecoder
from transformer_encoder import TransformerEncoder


class TransformerModel:
    def __init__(self, source_vectorization, target_vectorization, model_path=None,
        sequence_length=50, vocab_size=15000, embed_dim=256, dense_dim=2048, num_heads=8, drop_out=.5):

        self.source_vectorization = self.load_vectorization(source_vectorization)
        self.target_vectorization = self.load_vectorization(target_vectorization)

        self.load_model(model_path)

        self.sequence_length = sequence_length
        self.vocab_size = vocab_size
        self.embed_dim = embed_dim
        self.dense_dim = dense_dim
        self.num_heads = num_heads
        self.drop_out = drop_out

    def load_vectorization(self, vectorization):
        if isinstance(vectorization, str):
            return load_vectorization_from_disk(vectorization)
        else:
            return vectorization

    def load_model(self, model_path):
        if model_path is None:
            return

        self.model = load_model(
            model_path,
            custom_objects={
                'PositionalEmbedding': PositionalEmbedding,
                'TransformerDecoder': TransformerDecoder,
                'TransformerEncoder': TransformerEncoder
            })

    def build_model(self, *args, **kwargs):
        encoder_inputs = tf.keras.Input(shape=(None,), dtype="int64", name="stripped")
        x = PositionalEmbedding(self.sequence_length, self.vocab_size, self.embed_dim)(encoder_inputs)
        encoder_outputs = TransformerEncoder(self.embed_dim, self.dense_dim, self.num_heads)(x)

        decoder_inputs = tf.keras.Input(shape=(None,), dtype="int64", name="original")
        x = PositionalEmbedding(self.sequence_length, self.vocab_size, self.embed_dim)(decoder_inputs)
        x = TransformerDecoder(self.embed_dim, self.dense_dim, self.num_heads)(x, encoder_outputs)
        if self.drop_out > 0:
            x = layers.Dropout(self.drop_out)(x)
        decoder_outputs = layers.Dense(self.vocab_size, activation="softmax")(x)
        self.model = tf.keras.Model([encoder_inputs, decoder_inputs], decoder_outputs)

        self.model.compile(*args, **kwargs)

    def fit(self, *args, **kwargs):
        if not hasattr(self, 'model') or self.model is None:
            raise TypeError('Please build the model or load a pre-trained model first')

        return self.model.fit(*args, **kwargs)

    def evaluate(self, *args, **kwargs):
        if not hasattr(self, 'model') or self.model is None:
            raise TypeError('Please build the model or load a pre-trained model first')

        rs = self.model.evaluate(*args, **kwargs)
        print(rs)

    def predict(self, input_sentence):
        if not hasattr(self, 'model') or self.model is None:
            raise TypeError('Please load a pre-trained model first')

        if not hasattr(self, 'original_vocal'):
            self.original_vocal = self.target_vectorization.get_vocabulary()
            self.original_index_lookup = dict(zip(range(len(self.original_vocal)), self.original_vocal))

        input_word = input_sentence.split()
        tokenized_input_sentence = self.source_vectorization([input_sentence])
        decoded_sentence = "[start]"
        for i in range(min(self.sequence_length, len(input_sentence.split()))):
            tokenized_target_sentence = self.target_vectorization(
                [decoded_sentence])[:, :-1]
            predictions = self.model(
                [tokenized_input_sentence, tokenized_target_sentence])
            sampled_token_index = np.argmax(predictions[0, i, :])
            sampled_token = self.original_index_lookup[sampled_token_index] if sampled_token_index != 1 else input_word[i]
            decoded_sentence += " " + sampled_token
        return decoded_sentence.replace('[start]', '')
