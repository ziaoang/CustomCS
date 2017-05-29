# encoding=utf-8

import sys
reload(sys)
sys.setdefaultencoding("utf8")

from flask import Flask
from flask import request
from flask_sqlalchemy import SQLAlchemy
import hashlib
import json


app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = 'mysql://root:webkdd@localhost/customcs'
db = SQLAlchemy(app)


def as_dict(model):
	result = {}
	for c in model.__table__.columns:
		key, value = c.name, getattr(model, c.name)
		if isinstance(value, datetime):
			result[key] = str(value)
		else:
			result[key] = value
	return result


def md5(src):
	m = hashlib.md5()
	m.update(src)
	return m.hexdigest()


class User(db.Model):
	id            = db.Column(db.Integer, primary_key=True)  # 用户ID
	username      = db.Column(db.String(128))                # 用户名
	password_hash = db.Column(db.String(128))                # hash之后的密码
	blood         = db.Column(db.Integer)                    # 血量
	ammo          = db.Column(db.Integer)                    # 弹药
	experience    = db.Column(db.Integer)                    # 经验
	
	def __init__(self, username, password_hash, blood, ammo, experience):
		self.username = username;
		self.password_hash = password_hash;
		self.blood = blood;
		self.ammo = ammo;
		self.experience = experience;


@app.route("/")
def hello():
	return "Hello World!"


@app.route("/login", methods=['POST'])
def login():
	username = request.form['username'];
	password = request.form['password'];
	password_hash = md5(password)
	user = User.query.filter_by(username=username, password_hash=password_hash).first()
	result = {}
	if user == None:
		result['status'] = 'fail'
		result['message'] = '用户密码不匹配'
	else:
		result['status'] = 'succ'
		result['id'] = user.id
		result['username'] = user.username
		result['blood'] = user.blood
		result['ammo'] = user.ammo
		result['experience'] = user.experience
	return json.dumps(result)


@app.route("/register", methods=['POST'])
def register():
	username = request.form['username'];
	password = request.form['password'];
	password_hash = md5(password)
	user = User.query.filter_by(username=username).first()
	result = {}
	if user == None:
		db.session.add(User(username, password_hash, 100, 100, 0))
		db.session.commit()
		user = User.query.filter_by(username=username).first()
		result['status'] = 'succ'
		result['id'] = user.id
		result['username'] = user.username
		result['blood'] = user.blood
		result['ammo'] = user.ammo
		result['experience'] = user.experience
	else:
		result['status'] = 'fail'
		result['message'] = '用户密码不匹配'
	return json.dumps(result)


if __name__ == "__main__":
	app.run()


