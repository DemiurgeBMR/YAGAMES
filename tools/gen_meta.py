#!/usr/bin/env python3
"""Создаёт .meta для файлов и папок в Assets/, у которых их ещё нет.

Нужно, когда файлы добавлены вне Unity (например, в облачной сессии Claude):
GUID фиксируются в git сразу, и у всех копий проекта они одинаковые.
"""
import os
import uuid

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "Assets")

TAIL = "  userData: \n  assetBundleName: \n  assetBundleVariant: \n"

def meta_for(path, guid):
    head = f"fileFormatVersion: 2\nguid: {guid}\n"
    if os.path.isdir(path):
        return head + "folderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n" + TAIL
    ext = os.path.splitext(path)[1].lower()
    if ext == ".cs":
        return head + ("MonoImporter:\n  externalObjects: {}\n  serializedVersion: 2\n  defaultReferences: []\n"
                       "  executionOrder: 0\n  icon: {instanceID: 0}\n") + TAIL
    if ext == ".asmdef":
        return head + "AssemblyDefinitionImporter:\n  externalObjects: {}\n" + TAIL
    # Остальное Unity дополнит настройками импорта по умолчанию, сохранив GUID.
    return head

created = 0
for dirpath, dirnames, filenames in os.walk(ROOT):
    dirnames[:] = [d for d in dirnames if not d.startswith(".")]
    for name in dirnames + filenames:
        if name.endswith(".meta") or name.startswith("."):
            continue
        path = os.path.join(dirpath, name)
        meta = path + ".meta"
        if not os.path.exists(meta):
            with open(meta, "w", newline="\n") as f:
                f.write(meta_for(path, uuid.uuid4().hex))
            created += 1
            print("meta:", os.path.relpath(path, ROOT))
print(f"Создано .meta: {created}")
