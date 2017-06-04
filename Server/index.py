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
	id             = db.Column(db.Integer, primary_key=True)  # 用户ID
	username       = db.Column(db.String(128))                # 用户名
	password_hash  = db.Column(db.String(128))                # hash之后的密码
	max_score      = db.Column(db.Integer)                    # 历史最高得分
	is_save        = db.Column(db.Boolean)                    # 是否是保存数据
	remain_time    = db.Column(db.Integer)                    # 剩余时间
	attack         = db.Column(db.Integer)                    # 攻击力
	range          = db.Column(db.Integer)                    # 攻击范围
	level          = db.Column(db.Integer)                    # 等级
	experience     = db.Column(db.Integer)                    # 经验
	max_level      = db.Column(db.Integer)                    # 最大等级
	max_experience = db.Column(db.Integer)                    # 最大经验
	score          = db.Column(db.Integer)                    # 得分
	shield         = db.Column(db.Integer)                    # 护甲
	max_shield     = db.Column(db.Integer)                    # 最大护甲
	blood          = db.Column(db.Integer)                    # 血量
	max_blood      = db.Column(db.Integer)                    # 最大血量
	ammo           = db.Column(db.Integer)                    # 弹药
	max_ammo       = db.Column(db.Integer)                    # 最大弹药
	charger        = db.Column(db.Integer)                    # 弹夹
	max_charger    = db.Column(db.Integer)                    # 最大弹夹


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
		result['status']   = 'fail'
		result['message']  = '用户密码不匹配'
	else:
		result['status']   = 'succ'
		result['username'] = user.username
		result['is_save']  = user.is_save
		if user.is_save:
			result['remain_time']    = user.remain_time
			result['attack']         = user.attack
			result['range']          = user.range
			result['level']          = user.level
			result['experience']     = user.experience
			result['max_level']      = user.max_level
			result['max_experience'] = user.max_experience
			result['score']          = user.score
			result['shield']         = user.shield
			result['max_shield']     = user.max_shield
			result['blood']          = user.blood
			result['max_blood']      = user.max_blood
			result['ammo']           = user.ammo
			result['max_ammo']       = user.max_ammo
			result['charger']        = user.charger
			result['max_charger']    = user.max_charger
	return json.dumps(result)


@app.route("/register", methods=['POST'])
def register():
	username = request.form['username'];
	password = request.form['password'];
	password_hash = md5(password)
	user = User.query.filter_by(username=username).first()
	result = {}
	if user == None:
		user = User()
		user.username      = username
		user.password_hash = password_hash
		user.max_score     = 0
		user.is_save       = False
		db.session.add(user)
		db.session.commit()
		result['status']  = 'succ'
		result['message'] = '注册成功'
	else:
		result['status']  = 'fail'
		result['message'] = '该用户已存在'
	return json.dumps(result)


@app.route("/save", methods=['POST'])
def save():
	username = request.form['username']
	user = User.query.filter_by(username=username).first()
	user.is_save        = True
	user.remain_time    = int(request.form['remain_time'])
	user.attack         = int(request.form['attack'])
	user.range          = int(request.form['range'])
	user.level          = int(request.form['level'])
	user.experience     = int(request.form['experience'])
	user.max_level      = int(request.form['max_level'])
	user.max_experience = int(request.form['max_experience'])
	user.score          = int(request.form['score'])
	user.shield         = int(request.form['shield'])
	user.max_shield     = int(request.form['max_shield'])
	user.blood          = int(request.form['blood'])
	user.max_blood      = int(request.form['max_blood'])
	user.ammo           = int(request.form['ammo'])
	user.max_ammo       = int(request.form['max_ammo'])
	user.charger        = int(request.form['charger'])
	user.max_charger    = int(request.form['max_charger'])
	db.session.commit()
	result = {}
	result['status']  = 'succ'
	result['message'] = '保存成功'
	return json.dumps(result)


@app.route("/end", methods=['POST'])
def end():
	username = request.form['username']
	score    = int(request.form['score'])
	user = User.query.filter_by(username=username).first()
	user.is_save = False
	user.max_score = max(user.max_score, score)
	db.session.commit()

	userList = User.query.order_by(User.max_score.desc())
	result = {}
	result['status']  = 'succ'
	result['message'] = '结束成功'
	result["top10"] = []
	for i in range(10):
		result["top10"].append({})
		result["top10"][-1]["username"] = userList[i].username
		result["top10"][-1]["max_score"] = userList[i].max_score
	result["rank"] = 0
	for user in userList:
		result["rank"] += 1
		if user.username == username:
			break
	return json.dumps(result)


if __name__ == "__main__":
	app.run()


