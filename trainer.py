# -*- coding: utf-8 -*-

import tensorflow as tf

from data_loader import (create_vectorizations, load_data, make_dataset,
                         save_vectorization, load_vectorization_from_disk)
from transformer_model import TransformerModel

batch_size = 64

file_path = 'dataset/old-newspaper-vietnamese.txt'
train_pairs, val_pairs, test_pairs = load_data(file_path, limit=None)

callbacks=[
    tf.keras.callbacks.ModelCheckpoint(
        filepath='result/restore_diacritic.keras',
        save_best_only='True',
        monitor='val_accuracy'
    )
]

def train_from_scratch():
    source_vectorization, target_vectorization = create_vectorizations(train_pairs)
    save_vectorization(source_vectorization, 'result/source_vectorization_layer.pkl')
    save_vectorization(target_vectorization, 'result/target_vectorization_layer.pkl')

    train_ds = make_dataset(train_pairs, source_vectorization, target_vectorization, batch_size)
    val_ds = make_dataset(val_pairs, source_vectorization, target_vectorization, batch_size)

    transformer = TransformerModel(source_vectorization=source_vectorization,
        target_vectorization=target_vectorization,
        dense_dim=8192, num_heads=8, drop_out=0)
    transformer.build_model(optimizer="rmsprop",
        loss="sparse_categorical_crossentropy",
        metrics=["accuracy"])
    transformer.fit(train_ds, epochs=50, validation_data=val_ds, callbacks=callbacks)

    test_ds = make_dataset(test_pairs, source_vectorization, target_vectorization, batch_size)
    transformer.evaluate(test_ds)

    print(transformer.predict('mot ngay troi nang mot ngay troi mua'))
    print(transformer.predict('toi sinh ra o ha noi'))
    print(transformer.predict('em con nho hay em da quen'))
    print(transformer.predict('ten toi la thai duong'))

def continue_training():
    source_vectorization = load_vectorization_from_disk('result/source_vectorization_layer_cont.pkl')
    target_vectorization = load_vectorization_from_disk('result/target_vectorization_layer_cont.pkl')

    train_ds = make_dataset(train_pairs, source_vectorization, target_vectorization, batch_size)
    val_ds = make_dataset(val_pairs, source_vectorization, target_vectorization, batch_size)

    transformer = TransformerModel(source_vectorization=source_vectorization,
        target_vectorization=target_vectorization,
        model_path='result/restore_diacritic_cont.keras')
    transformer.fit(train_ds, epochs=5, validation_data=val_ds, callbacks=callbacks)

    test_ds = make_dataset(test_pairs, source_vectorization, target_vectorization, batch_size)
    transformer.evaluate(test_ds)

    print(transformer.predict('mot ngay troi nang mot ngay troi mua'))
    print(transformer.predict('toi sinh ra o ha noi'))
    print(transformer.predict('em con nho hay em da quen'))
    print(transformer.predict('ten toi la thai duong'))
